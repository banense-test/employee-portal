namespace EmployeePortal.Infrastructure;

/// <summary>
/// LDAP connection settings. The production service-account values land with R010 (Construction);
/// the disposable directory supplies its own settings for the Elaboration validation.
/// </summary>
public sealed record LdapConnectionSettings(string Host, int Port, string? BindDn = null, string? BindPassword = null, TimeSpan? Timeout = null)
{
    /// <summary>PRF-003: 5 s hard timeout (overridable so the timeout mechanism is testable without burning 5 s).</summary>
    public TimeSpan EffectiveTimeout => Timeout ?? TimeSpan.FromSeconds(5);
}

/// <summary>
/// Raw entry from the directory — attribute values exactly as stored; missing attributes are absent
/// from the dictionary (the gateway maps them to null, never to a substituted value).
/// </summary>
public sealed class LdapRawEntry(string uid)
{
    private readonly Dictionary<string, string> _attributes = new(StringComparer.OrdinalIgnoreCase);

    public string Uid { get; } = uid;

    public void Set(string attribute, string? value)
    {
        if (value is not null) _attributes[attribute] = value;
    }

    public string? GetAttribute(string attribute)
        => _attributes.TryGetValue(attribute, out var value) ? value : null;
}

/// <summary>Wire-level LDAP failure (unreachable / protocol error) — translated by the gateway to DirectoryUnavailableException.</summary>
public sealed class LdapConnectionException(string message, Exception? inner = null) : Exception(message, inner);

/// <summary>
/// The LDAP wire seam (COMP-007 volatility point). The production adapter (client library + service
/// account, R010/R011) lands at Construction integration; the disposable LDAP directory (the Elaboration
/// validation fixture) implements the same seam. ARCH-8: read-only — the seam exposes NO write
/// operation, so the gateway cannot write to Active Directory (CON-007).
/// </summary>
public interface ILdapConnection
{
    Task<IReadOnlyList<LdapRawEntry>> SearchAsync(string filter, IReadOnlyList<string> attributes, CancellationToken cancellationToken);
}
