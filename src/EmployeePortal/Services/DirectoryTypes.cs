namespace EmployeePortal.Services;

/// <summary>
/// CLS-026 DirectoryEntry — transient AD projection (six corporate fields, FR-010); never persisted
/// (CON-006). Missing attributes are null — displayed as missing, NEVER substituted (R001 clause d).
/// </summary>
public sealed record DirectoryEntry(string? DisplayName, string? JobTitle, string? Department, string? Office, string? Email, string? Extension);

/// <summary>CLS-027 EmployeeDisplayData — transient display projection for the HR views (UC-005/006/007).</summary>
public sealed record EmployeeDisplayData(string? DisplayName, string? Department, string? Office);

/// <summary>UC-004 search criteria — all components optional.</summary>
public sealed record DirectorySearchCriteria(string? Name = null, string? Department = null, string? Office = null);

public sealed record DirectoryResult(IReadOnlyList<DirectoryEntry> Entries);

/// <summary>
/// AD unreachable / query timeout — a DISTINCT condition from attribute gaps (AF-2 vs AF-3):
/// the behavioural bar does not waive the AF-2 contract (uid-only table for UC-005, abort for
/// UC-006, blocked lookup for UC-007).
/// </summary>
public sealed class DirectoryUnavailableException(string message, Exception? inner = null) : Exception(message, inner);
