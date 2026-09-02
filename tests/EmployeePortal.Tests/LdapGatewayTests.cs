using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using EmployeePortal.Tests.Fixtures;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>White-box coverage for CLS-009 LdapGateway — filter construction, attribute mapping,
/// escaping, timeout, failure translation, D-9 completeness, and the read-only seam.</summary>
public class LdapGatewayTests
{
    private static (LdapGateway Gateway, DisposableLdapDirectory Directory) Create(TimeSpan? timeout = null)
    {
        var directory = new DisposableLdapDirectory();
        var settings = new LdapConnectionSettings("disposable-directory", 389, Timeout: timeout);
        return (new LdapGateway(directory, settings), directory);
    }

    [Fact]
    public async Task Search_WithNoCriteria_ReturnsEverySeededEntry()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria());
        Assert.Equal(11, result.Count); // clause (a) at the mechanism level: nothing is filtered away
    }

    [Fact]
    public async Task Search_ByName_ReturnsAllMatches_IncludingEntriesWithMissingAttributes()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria(Name: "Gomez"));
        Assert.Equal(4, result.Count); // Maria, Luis, Ana, "Gomez, Maria Clara" — gapped entries NOT removed (clause b)
    }

    [Fact]
    public async Task Search_ByDepartment_FiltersExactly()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria(Department: "Finance"));
        Assert.Equal(3, result.Count); // Maria, Luis, Maria Clara
    }

    [Fact]
    public async Task Search_ByOffice_FiltersExactly()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria(Office: "North"));
        // Pablo + Eva. John Perez's office is deliberately MISSING (substitution-attempt fixture):
        // an office filter legitimately excludes entries with no office attribute — that is query
        // semantics, not the behavioural bar. The bar (SEQ-004 AF-2) governs the rendering of
        // RETURNED entries, never the filter's match set.
        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.DisplayName == "Pablo Ruiz");
        Assert.Contains(result, e => e.DisplayName == "Eva Ruiz");
    }

    [Fact]
    public async Task Search_CombinedCriteria_Intersects()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria(Name: "Gomez", Office: "Central"));
        Assert.Equal(2, result.Count); // Maria, Luis
    }

    [Fact]
    public async Task Search_ByNameWithParentheses_MatchesEscapedFilter()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria(Name: "Cross (South)"));
        var nora = Assert.Single(result);
        Assert.Equal("Nora Cross (South)", nora.DisplayName); // LDAP filter escaping round-trip
    }

    [Fact]
    public async Task Search_EmptyStringAttributeValue_IsMissing()
    {
        var (gateway, _) = Create();
        var result = await gateway.SearchAsync(new DirectorySearchCriteria(Name: "Sara"));
        var sara = Assert.Single(result);
        Assert.Null(sara.Department); // an empty AD value is a missing value — never rendered as ""
    }

    [Fact]
    public async Task Search_DirectoryFailure_ThrowsDirectoryUnavailable()
    {
        var (gateway, directory) = Create();
        directory.FailureMode = DirectoryFailureMode.ConnectionFailed;

        await Assert.ThrowsAsync<DirectoryUnavailableException>(() => gateway.SearchAsync(new DirectorySearchCriteria()));
    }

    [Fact]
    public async Task Search_QueryTimeout_ThrowsDirectoryUnavailable()
    {
        // PRF-003: the 5 s hard timeout mechanism, exercised with a shortened timeout so the test
        // does not burn 5 seconds — the mechanism (cancel -> translate) is identical.
        var (gateway, directory) = Create(TimeSpan.FromMilliseconds(50));
        directory.FailureMode = DirectoryFailureMode.Timeout;

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        await Assert.ThrowsAsync<DirectoryUnavailableException>(() => gateway.SearchAsync(new DirectorySearchCriteria()));
        stopwatch.Stop();
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5), $"The hard timeout did not fire: {stopwatch.Elapsed}");
    }

    [Fact]
    public async Task GetDisplayData_MapIsCompleteOverRequestedUids()
    {
        var (gateway, _) = Create();

        var map = await gateway.GetDisplayDataAsync(["u003", "u999"]);

        Assert.Equal(2, map.Count); // D-9: the unresolvable uid is present with an all-null entry
        Assert.Equal("Ana Gomez", map["u003"].DisplayName);
        Assert.Null(map["u003"].Department);
        Assert.Null(map["u003"].Office);
        Assert.Null(map["u999"].DisplayName);
        Assert.Null(map["u999"].Department);
        Assert.Null(map["u999"].Office);
    }

    [Fact]
    public async Task GetDisplayData_EmptyUidList_ReturnsEmptyMap()
    {
        var (gateway, _) = Create();
        var map = await gateway.GetDisplayDataAsync([]);
        Assert.Empty(map);
    }

    [Fact]
    public async Task GetDisplayData_DuplicateUids_ResolvedOnce()
    {
        var (gateway, _) = Create();
        var map = await gateway.GetDisplayDataAsync(["u001", "u001"]);
        Assert.Single(map);
    }

    [Fact]
    public void LdapConnectionSeam_IsReadOnly()
    {
        // ARCH-8: the compiler enforces read-only LDAP (CON-007) — the seam exposes no write operation.
        Assert.All(typeof(ILdapConnection).GetMethods(),
            m => Assert.True(m.Name.StartsWith("Search", StringComparison.Ordinal),
                $"The LDAP seam must be read-only; found mutating method: {m.Name}"));
    }
}
