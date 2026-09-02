using System.Text;
using EmployeePortal.Services;

namespace EmployeePortal.Infrastructure;

/// <summary>
/// CLS-009 LdapGateway (COMP-007) — LDAP query construction, connection management, result mapping.
/// R001 volatility point: query strategy changes touch this class only.
/// FOUR-clause graceful degradation (stakeholder decisions, Elab Iter 2 + verdict-gate contribution):
/// (a) every entry is mapped — none dropped; (b) a missing attribute never removes an entry;
/// (c) a missing attribute never raises an error; (d) a missing attribute maps to null — the FINAL
/// value, never a default, a placeholder, a guessed value, or another employee's value.
/// </summary>
public sealed class LdapGateway(ILdapConnection connection, LdapConnectionSettings settings) : ILdapGateway
{
    private const string ObjectClass = "person";
    private const string UidAttribute = "uid";
    private const string CommonName = "cn";
    private const string TitleAttribute = "title";
    private const string DepartmentAttribute = "department";
    private const string OfficeAttribute = "physicalDeliveryOfficeName";
    private const string MailAttribute = "mail";
    private const string ExtensionAttribute = "telephoneNumber";

    private static readonly string ObjectClassFilter = $"(objectClass={ObjectClass})";

    private static readonly IReadOnlyList<string> SearchAttributes =
        [CommonName, TitleAttribute, DepartmentAttribute, OfficeAttribute, MailAttribute, ExtensionAttribute];

    private static readonly IReadOnlyList<string> DisplayAttributes =
        [UidAttribute, CommonName, DepartmentAttribute, OfficeAttribute];

    public async Task<IReadOnlyList<DirectoryEntry>> SearchAsync(DirectorySearchCriteria criteria)
    {
        var filter = BuildFilter(criteria);
        var raw = await QueryAsync(filter, SearchAttributes);
        return raw.Select(MapEntry).ToList(); // clause (a): every entry mapped — none dropped
    }

    public async Task<IReadOnlyDictionary<string, EmployeeDisplayData>> GetDisplayDataAsync(IEnumerable<string> uids)
    {
        var distinct = uids.Distinct(StringComparer.Ordinal).ToList();
        var map = new Dictionary<string, EmployeeDisplayData>(StringComparer.Ordinal);
        if (distinct.Count == 0) return map;

        var filter = "(&" + ObjectClassFilter + "(|" +
                     string.Concat(distinct.Select(u => $"({UidAttribute}={Escape(u)})")) + "))";
        var raw = await QueryAsync(filter, DisplayAttributes);

        var byUid = new Dictionary<string, EmployeeDisplayData>(StringComparer.Ordinal);
        foreach (var entry in raw)
            byUid[entry.Uid] = MapDisplayEntry(entry);

        foreach (var uid in distinct)
            map[uid] = byUid.TryGetValue(uid, out var data)
                ? data
                : new EmployeeDisplayData(null, null, null); // D-9: unresolvable uid -> all-null entry, NEVER omitted
        return map;
    }

    private async Task<IReadOnlyList<LdapRawEntry>> QueryAsync(string filter, IReadOnlyList<string> attributes)
    {
        using var cts = new CancellationTokenSource(settings.EffectiveTimeout); // 5 s hard timeout — PRF-003
        try
        {
            return await connection.SearchAsync(filter, attributes, cts.Token);
        }
        catch (OperationCanceledException ex) // the hard timeout fired
        {
            throw new DirectoryUnavailableException("The directory query timed out.", ex);
        }
        catch (LdapConnectionException ex)
        {
            throw new DirectoryUnavailableException("The directory is temporarily unavailable.", ex);
        }
    }

    private static string BuildFilter(DirectorySearchCriteria criteria)
    {
        var parts = new List<string> { ObjectClassFilter };
        if (!string.IsNullOrWhiteSpace(criteria.Name))
            parts.Add($"({CommonName}=*{Escape(criteria.Name)}*)");
        if (!string.IsNullOrWhiteSpace(criteria.Department))
            parts.Add($"({DepartmentAttribute}={Escape(criteria.Department)})");
        if (!string.IsNullOrWhiteSpace(criteria.Office))
            parts.Add($"({OfficeAttribute}={Escape(criteria.Office)})");
        return parts.Count == 1 ? parts[0] : "(&" + string.Concat(parts) + ")";
    }

    private static DirectoryEntry MapEntry(LdapRawEntry raw)
        => new(Get(raw, CommonName), Get(raw, TitleAttribute), Get(raw, DepartmentAttribute),
               Get(raw, OfficeAttribute), Get(raw, MailAttribute), Get(raw, ExtensionAttribute));

    private static EmployeeDisplayData MapDisplayEntry(LdapRawEntry raw)
        => new(Get(raw, CommonName), Get(raw, DepartmentAttribute), Get(raw, OfficeAttribute));

    /// <summary>
    /// Missing or empty AD value -> null. Null is the FINAL mapped value (clause d): blank is an answer —
    /// never a default, a placeholder, a guessed value, or another employee's value.
    /// </summary>
    private static string? Get(LdapRawEntry raw, string attribute)
    {
        var value = raw.GetAttribute(attribute);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    /// <summary>LDAP filter escaping (RFC 4515).</summary>
    private static string Escape(string value)
    {
        var sb = new StringBuilder(value.Length);
        foreach (var c in value)
        {
            switch (c)
            {
                case '*': sb.Append("\\2a"); break;
                case '(': sb.Append("\\28"); break;
                case ')': sb.Append("\\29"); break;
                case '\\': sb.Append("\\5c"); break;
                case '\0': sb.Append("\\00"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }
}
