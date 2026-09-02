using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using Xunit;

namespace EmployeePortal.Tests;

public class DirectoryServiceTests
{
    private static DirectoryService Create(ILdapGateway gateway) => new(gateway);

    private sealed class FakeGateway(IReadOnlyList<DirectoryEntry> entries, IReadOnlyDictionary<string, EmployeeDisplayData> display)
        : ILdapGateway
    {
        public Task<IReadOnlyList<DirectoryEntry>> SearchAsync(DirectorySearchCriteria criteria)
            => Task.FromResult(entries);

        public Task<IReadOnlyDictionary<string, EmployeeDisplayData>> GetDisplayDataAsync(IEnumerable<string> uids)
            => Task.FromResult(display);
    }

    [Fact]
    public async Task Search_DelegatesToGateway()
    {
        var entries = new List<DirectoryEntry> { new("Maria Gomez", "Analyst", "Finance", "Central", "m@c.example", "2451") };
        var service = Create(new FakeGateway(entries, new Dictionary<string, EmployeeDisplayData>()));

        var result = await service.SearchAsync(new DirectorySearchCriteria(Name: "Maria"));

        Assert.Same(entries, result.Entries);
    }

    [Fact]
    public async Task GetDisplayData_GatewayOmitsUid_ServiceFillsAllNullEntry()
    {
        // D-9 defensive completeness: even a gateway that omits a uid never removes it from the map.
        var display = new Dictionary<string, EmployeeDisplayData>
        {
            ["u001"] = new("Maria Gomez", "Finance", "Central"),
        };
        var service = Create(new FakeGateway([], display));

        var result = await service.GetDisplayDataAsync(["u001", "u999"]);

        Assert.Equal(2, result.Count);
        Assert.True(result.ContainsKey("u999"));
        Assert.Null(result["u999"].DisplayName);
        Assert.Null(result["u999"].Department);
        Assert.Null(result["u999"].Office);
    }

    [Fact]
    public async Task GetDisplayData_DuplicateUids_ResolvedOnce()
    {
        var service = Create(new FakeGateway([], new Dictionary<string, EmployeeDisplayData>()));

        var result = await service.GetDisplayDataAsync(["u001", "u001", "u001"]);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetDisplayData_EmptyUidList_ReturnsEmptyMap()
    {
        var service = Create(new FakeGateway([], new Dictionary<string, EmployeeDisplayData>()));

        var result = await service.GetDisplayDataAsync([]);

        Assert.Empty(result);
    }
}
