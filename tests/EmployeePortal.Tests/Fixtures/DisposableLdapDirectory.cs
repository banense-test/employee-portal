using System.Net;
using System.Net.Sockets;
using System.Text;
using EmployeePortal.Infrastructure;

namespace EmployeePortal.Tests.Fixtures;

public enum DirectoryFailureMode { None, ConnectionFailed, Timeout }

/// <summary>
/// The R001 disposable LDAP directory (stakeholder decision, Elab Iter 1: "Stand one up, disposable,
/// and answer it empirically this phase"). NOT the production AD — no STK-004 dependency (R010 blocks
/// production-instance integration only, in Construction).
///
/// Seeded DELIBERATELY (a fixture that cannot fail proves nothing):
/// - 3 offices (fixture labels — the declared input names no office locations);
/// - attribute gaps across ALL SIX corporate attributes;
/// - substitution-attempt fixtures: a missing department that tempts a "General" default, a missing
///   office that tempts a first-office ("Central") fallback, a missing job title that tempts an "N/A"
///   placeholder — so clause (d) of the FOUR-clause behavioural bar can actually fail;
/// - an entry with an EMPTY-STRING attribute (an empty AD value is a missing value);
/// - an entry whose name contains parentheses (LDAP filter escaping round-trip);
/// - u999 is NEVER seeded — the unresolvable-uid fixture (D-9: a departed employee with clocking history).
/// Retained as a reusable Construction test asset (R011 residual).
/// </summary>
public sealed class DisposableLdapDirectory : ILdapConnection
{
    private readonly List<LdapRawEntry> _entries = new();

    public DirectoryFailureMode FailureMode { get; set; } = DirectoryFailureMode.None;

    /// <summary>How long the "timeout" failure holds the connection open — the gateway's hard timeout cancels first.</summary>
    public TimeSpan FailureDelay { get; set; } = TimeSpan.FromSeconds(10);

    public DisposableLdapDirectory() => Seed();

    public async Task<IReadOnlyList<LdapRawEntry>> SearchAsync(string filter, IReadOnlyList<string> attributes, CancellationToken cancellationToken)
    {
        switch (FailureMode)
        {
            case DirectoryFailureMode.ConnectionFailed:
                throw new LdapConnectionException("Simulated directory failure: the directory is unreachable.");
            case DirectoryFailureMode.Timeout:
                await Task.Delay(FailureDelay, cancellationToken); // the gateway's hard timeout cancels this
                break;
        }
        return _entries.Where(e => FilterMatches(e, filter)).ToList();
    }

    private void Seed()
    {
        // Office "Central" (fixture label)
        Add("u001", "Maria Gomez", "Financial Analyst", "Finance", "Central", "m.gomez@cubacorp.example", "2451");
        Add("u002", "Luis Gomez", null, "Finance", "Central", "l.gomez@cubacorp.example", null); // missing job title + extension
        // SUBSTITUTION ATTEMPTS: missing department (a naive default would render "General") +
        // missing office (a naive fallback would render the first office, "Central")
        Add("u003", "Ana Gomez", "HR Generalist", null, null, "a.gomez@cubacorp.example", "2453");

        // Office "North" (fixture label)
        Add("u004", "Pablo Ruiz", "IT Engineer", "IT", "North", "p.ruiz@cubacorp.example", "3101");
        Add("u005", "Eva Ruiz", "Accountant", null, "North", null, "3102"); // missing department + email
        // SUBSTITUTION ATTEMPTS: missing office (tempts "Central") + missing job title (tempts the "N/A" placeholder)
        Add("u006", "John Perez", null, "Logistics", null, "j.perez@cubacorp.example", "3103");

        // Office "South" (fixture label)
        Add("u007", "Lucia Diaz", "Office Manager", "Operations", "South", "l.diaz@cubacorp.example", "4201");
        Add("u008", "Marco Diaz", null, null, null, null, null); // D-9 extreme: only uid + name present
        Add("u009", "Sara Vega", "Analyst", "", "South", "s.vega@cubacorp.example", "4202"); // EMPTY-STRING department = missing value
        Add("u010", "Nora Cross (South)", "Receptionist", "Operations", "South", "n.cross@cubacorp.example", "4203"); // parentheses — filter escaping round-trip
        Add("u011", "Gomez, Maria Clara", "Auditor", "Finance", "South", "mc.gomez@cubacorp.example", "4204"); // comma in name — CSV quoting
    }

    private void Add(string uid, string? cn, string? title, string? department, string? office, string? mail, string? extension)
    {
        var entry = new LdapRawEntry(uid);
        entry.Set("objectClass", "person");
        entry.Set("uid", uid);
        if (cn is not null) entry.Set("cn", cn);
        if (title is not null) entry.Set("title", title);
        if (department is not null) entry.Set("department", department);
        if (office is not null) entry.Set("physicalDeliveryOfficeName", office);
        if (mail is not null) entry.Set("mail", mail);
        if (extension is not null) entry.Set("telephoneNumber", extension);
        _entries.Add(entry);
    }

    // ---- minimal filter evaluator for the shapes LdapGateway produces ----

    private static bool FilterMatches(LdapRawEntry entry, string filter)
    {
        filter = filter.Trim();
        if (filter.StartsWith("(&", StringComparison.Ordinal) && filter.EndsWith(")", StringComparison.Ordinal))
        {
            foreach (var part in SplitGroups(filter[2..^1]))
                if (!FilterMatches(entry, part)) return false;
            return true;
        }
        if (filter.StartsWith("(|", StringComparison.Ordinal) && filter.EndsWith(")", StringComparison.Ordinal))
        {
            foreach (var part in SplitGroups(filter[2..^1]))
                if (FilterMatches(entry, part)) return true;
            return false;
        }

        var inner = filter[1..^1]; // (attr=value)
        var eq = inner.IndexOf('=');
        if (eq < 0) return false;
        var attribute = inner[..eq];
        var pattern = Unescape(inner[(eq + 1)..]);
        var value = entry.GetAttribute(attribute);
        return value is not null && MatchesWildcard(value, pattern);
    }

    private static IEnumerable<string> SplitGroups(string filter)
    {
        var depth = 0;
        var start = 0;
        for (var i = 0; i < filter.Length; i++)
        {
            if (filter[i] == '(') { if (depth == 0) start = i; depth++; }
            else if (filter[i] == ')') { depth--; if (depth == 0) yield return filter[start..(i + 1)]; }
        }
    }

    private static string Unescape(string value)
    {
        var sb = new StringBuilder(value.Length);
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '\\' && i + 2 < value.Length)
            {
                var hi = HexValue(value[i + 1]);
                var lo = HexValue(value[i + 2]);
                if (hi >= 0 && lo >= 0) { sb.Append((char)(hi * 16 + lo)); i += 2; continue; }
            }
            sb.Append(value[i]);
        }
        return sb.ToString();
    }

    private static int HexValue(char c) => c switch
    {
        >= '0' and <= '9' => c - '0',
        >= 'a' and <= 'f' => c - 'a' + 10,
        >= 'A' and <= 'F' => c - 'A' + 10,
        _ => -1
    };

    private static bool MatchesWildcard(string value, string pattern)
    {
        var parts = pattern.Split('*');
        var pos = 0;
        for (var i = 0; i < parts.Length; i++)
        {
            if (parts[i].Length == 0) continue;
            var idx = value.IndexOf(parts[i], pos, StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return false;
            if (i == 0 && idx != 0) return false; // pattern starts with a literal -> must match at the start
            pos = idx + parts[i].Length;
        }
        if (parts[^1].Length > 0 && pos != value.Length) return false; // ends with a literal -> must match at the end
        return true;
    }
}
