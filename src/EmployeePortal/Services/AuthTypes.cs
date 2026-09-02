namespace EmployeePortal.Services;

/// <summary>
/// The authenticated identity at the request boundary — Uid + roles from claims (SEC-002).
/// Produced by CLS-010 (Infrastructure); consumed by every controller (Presentation).
/// </summary>
public sealed record AuthenticatedUser(string Uid, IReadOnlySet&lt;string&gt; Roles);

/// <summary>
/// SEC-006 role names as they appear in the Keycloak realm_access.roles claim. The realm
/// configuration lands with the Keycloak client registration (R010, Construction); the stub
/// issuer mints the same names for the Elaboration validation.
/// </summary>
public static class RoleNames
{
    public const string Employee = "employee";
    public const string HrAdministrator = "hr_administrator";
}
