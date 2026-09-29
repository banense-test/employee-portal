namespace EmployeePortal.Infrastructure;

/// <summary>
/// Implementation View mapping for the infrastructure layer — the mechanisms.
/// Each external system has exactly one adapter, so a change in how it is
/// reached touches one component.
/// </summary>
public static class InfrastructureComponentMap
{
    /// <summary>COMP-006 Audit Trail — INT-006 IAuditTrail. NFR-002.</summary>
    public const string AuditTrail = "EmployeePortal.Infrastructure.AuditTrail";

    /// <summary>COMP-007 Identity and Authorization — INT-007 IIdentityContext. CON-001, CON-031, CON-033.</summary>
    public const string Identity = "EmployeePortal.Infrastructure.Identity";

    /// <summary>COMP-008 Active Directory Adapter — INT-008 IEmployeeDirectory. CON-003, CON-011. The only reader of AD.</summary>
    public const string DirectoryAdapter = "EmployeePortal.Infrastructure.DirectoryAdapter";

    /// <summary>COMP-009 Persistence — INT-009 IRepository. CON-006, CON-030.</summary>
    public const string Persistence = "EmployeePortal.Infrastructure.Persistence";

    /// <summary>The four infrastructure components, in dependency order.</summary>
    public static IReadOnlyList<string> All { get; } =
    [
        Persistence,
        AuditTrail,
        Identity,
        DirectoryAdapter,
    ];
}
