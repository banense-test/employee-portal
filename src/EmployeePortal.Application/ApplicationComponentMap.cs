namespace EmployeePortal.Application;

/// <summary>
/// Implementation View mapping for the application and domain layer.
/// The source layout follows the component boundaries fixed by the Software
/// Architecture Document (Logical View): one namespace per component, because
/// the component — not the feature — is the unit of change.
/// </summary>
public static class ApplicationComponentMap
{
    /// <summary>COMP-001 Clocking Capture — INT-001 IClockingService. CON-017, CON-043, CON-044.</summary>
    public const string ClockingCapture = "EmployeePortal.Application.ClockingCapture";

    /// <summary>COMP-002 Clocking Report and CSV Export — INT-002 IClockingReport. CON-007, CON-008.</summary>
    public const string ClockingReport = "EmployeePortal.Application.ClockingReport";

    /// <summary>COMP-003 News — INT-003 INewsService. CON-019, CON-020, CON-021, CON-022, CON-023.</summary>
    public const string News = "EmployeePortal.Application.News";

    /// <summary>COMP-004 Directory Search — INT-004 IDirectorySearch. FR-009, CON-024, CON-027.</summary>
    public const string DirectorySearch = "EmployeePortal.Application.DirectorySearch";

    /// <summary>COMP-005 Worker Category — INT-005 IWorkerCategory. CON-004, CON-025, CON-026.</summary>
    public const string WorkerCategory = "EmployeePortal.Application.WorkerCategory";

    /// <summary>The five application and domain components, in dependency order.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        ClockingCapture,
        ClockingReport,
        News,
        DirectorySearch,
        WorkerCategory,
    ];
}
