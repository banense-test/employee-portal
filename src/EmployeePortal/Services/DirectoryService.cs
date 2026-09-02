using EmployeePortal.Infrastructure;

namespace EmployeePortal.Services;

/// <summary>
/// CLS-003 DirectoryService (COMP-003) — delegates queries to the LDAP gateway: the single read path
/// shared by all four AD-reading use cases (UC-004/005/006/007), so the R001 behavioural bar holds in
/// every consumer through one mechanism. D-9: GetDisplayData returns a map COMPLETE over the requested
/// uid set — a uid AD cannot resolve maps to an all-null EmployeeDisplayData.
/// </summary>
public sealed class DirectoryService(ILdapGateway ldap) : IDirectoryService
{
    public async Task<DirectoryResult> SearchAsync(DirectorySearchCriteria criteria)
        => new(await ldap.SearchAsync(criteria));

    public async Task<IReadOnlyDictionary<string, EmployeeDisplayData>> GetDisplayDataAsync(IEnumerable<string> uids)
    {
        var distinct = uids.Distinct(StringComparer.Ordinal).ToList();
        var fromLdap = await ldap.GetDisplayDataAsync(distinct);
        var result = new Dictionary<string, EmployeeDisplayData>(fromLdap);
        // D-9 (defensive): the gateway guarantees completeness; if an entry were ever missing, fill
        // all-null — a uid is NEVER omitted from the map (clause a: every employee is rendered).
        foreach (var uid in distinct)
            if (!result.ContainsKey(uid))
                result[uid] = new EmployeeDisplayData(null, null, null);
        return result;
    }
}
