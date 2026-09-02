namespace EmployeePortal.Services;

/// <summary>
/// INT-008 IDirectoryService (COMP-003) — directory search + display-data resolution.
/// Postconditions carry the R001 behavioural bar, FOUR clauses (stakeholder-confirmed, Elab Iter 2
/// + verdict-gate contribution): (a) every employee is rendered whether or not their attributes are
/// complete; (b) a missing attribute never removes someone from results; (c) a missing attribute never
/// raises an error; (d) a missing attribute is displayed as missing — never replaced by a default,
/// a placeholder, a guessed value, or another employee's value.
/// </summary>
public interface IDirectoryService
{
    Task<DirectoryResult> SearchAsync(DirectorySearchCriteria criteria);
    Task<IReadOnlyDictionary<string, EmployeeDisplayData>> GetDisplayDataAsync(IEnumerable<string> uids);
}
