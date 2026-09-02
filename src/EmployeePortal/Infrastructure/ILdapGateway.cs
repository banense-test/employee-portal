using EmployeePortal.Services;

namespace EmployeePortal.Infrastructure;

/// <summary>
/// INT-010 ILdapGateway (COMP-007) — read-only LDAP v3 (CON-007), live query on demand (CON-006),
/// 5 s hard timeout (PRF-003). R001 behavioural bar, FOUR clauses, one contract, four consumers
/// (UC-004/005/006/007): null is the FINAL mapped value for a missing attribute — never substituted
/// by a default, a placeholder, a guessed value, or another employee's value.
/// </summary>
public interface ILdapGateway
{
    Task<IReadOnlyList<DirectoryEntry>> SearchAsync(DirectorySearchCriteria criteria);
    Task<IReadOnlyDictionary<string, EmployeeDisplayData>> GetDisplayDataAsync(IEnumerable<string> uids);
}
