using System.Text;
using EmployeePortal.Infrastructure;

namespace EmployeePortal.Services;

/// <summary>INT-013 IReportExport (COMP-010) — encapsulates the CSV column set v1 (Medium volatility).</summary>
public interface IReportExport
{
    Task<ExportResult> ExportMonthAsync(int year, int month);
}

/// <summary>ExportResult: Success | NoData | Aborted.DirectoryUnavailable (NO partial file — UC-006 AF-2).</summary>
public abstract record ExportResult
{
    public sealed record Success(byte[] CsvBytes, string FileName) : ExportResult;
    public sealed record NoData() : ExportResult;
    public sealed record AbortedDirectoryUnavailable() : ExportResult;
}

/// <summary>
/// CLS-006 ReportExportService (COMP-010) — CSV column set v1 (ad_user_id, employee_name, department,
/// office, event_timestamp, event_type); month boundaries computed as local calendar days in
/// America/Havana (payroll day = local day, never the server's); missing display attributes render as
/// TRULY EMPTY cells — no placeholder character, no abort, NEVER a substituted value (UC-006 AF-3,
/// R001 clause d: the CSV reaches payroll — an empty cell gets questioned, a plausible wrong one does not).
/// </summary>
public sealed class ReportExportService(IClockingsRepository clockings, IDirectoryService directory, ITimeConvention time) : IReportExport
{
    private const string Header = "ad_user_id,employee_name,department,office,event_timestamp,event_type";

    public async Task<ExportResult> ExportMonthAsync(int year, int month)
    {
        var bounds = time.MonthBoundsLocal(year, month);
        var events = await clockings.GetByRangeAsync(bounds.FromUtc, bounds.ToUtc);
        if (events.Count == 0) return new ExportResult.NoData();

        IReadOnlyDictionary<string, EmployeeDisplayData> display;
        try
        {
            display = await directory.GetDisplayDataAsync(events.Select(e => e.EmployeeUid));
        }
        catch (DirectoryUnavailableException)
        {
            return new ExportResult.AbortedDirectoryUnavailable(); // AF-2: abort — NO partial file
        }

        var rows = new List<string> { Header };
        foreach (var @event in events) // clause (a): EVERY event row written — no row dropped for missing display data
        {
            display.TryGetValue(@event.EmployeeUid, out var data); // D-9: the map is complete over the requested uids
            rows.Add(string.Join(",",
                Csv(@event.EmployeeUid),
                Csv(data?.DisplayName),
                Csv(data?.Department),
                Csv(data?.Office),
                Csv(time.ToIso8601WithOffset(@event.RecordedAtUtc)),
                Csv(@event.EventType == ClockingEventType.In ? "in" : "out")));
        }

        var csv = string.Join("\r\n", rows) + "\r\n";
        return new ExportResult.Success(Encoding.UTF8.GetBytes(csv), $"clocking-report-{year}-{month:D2}.csv");
    }

    /// <summary>null -> a truly EMPTY cell (clause d). Values containing commas/quotes/newlines are RFC 4180-quoted.</summary>
    private static string Csv(string? value)
    {
        if (value is null) return string.Empty;
        var needsQuoting = value.Contains(',') || value.Contains('"') || value.Contains('\r') || value.Contains('\n');
        return needsQuoting ? "\"" + value.Replace("\"", "\"\"") + "\"" : value;
    }
}
