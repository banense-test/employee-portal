using System.Text;
using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using EmployeePortal.Tests.Fixtures;
using Xunit;

namespace EmployeePortal.Tests;

public class ReportExportServiceTests
{
    private static ReportExportService Create(IReadOnlyList<ClockingEvent> events, DirectoryFailureMode failureMode = DirectoryFailureMode.None)
    {
        var directory = new DisposableLdapDirectory { FailureMode = failureMode };
        var gateway = new LdapGateway(directory, new LdapConnectionSettings("disposable-directory", 389));
        return new ReportExportService(new FakeClockingsRepository(events), new DirectoryService(gateway), new TimeService());
    }

    private static ClockingEvent Event(string uid, ClockingEventType type, DateTimeOffset recordedAtUtc) => new()
    {
        EmployeeUid = uid,
        EventType = type,
        RecordedAtUtc = recordedAtUtc,
        IdempotencyKey = $"key-{uid}-{recordedAtUtc:O}",
    };

    private static string[] Lines(ExportResult.Success success)
        => Encoding.UTF8.GetString(success.CsvBytes).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

    [Fact]
    public async Task ExportMonth_NoEvents_ReturnsNoData()
    {
        var export = await Create([]).ExportMonthAsync(2026, 9);
        Assert.IsType<ExportResult.NoData>(export);
    }

    [Fact]
    public async Task ExportMonth_DirectoryUnavailable_AbortsWithoutPartialFile()
    {
        var export = await Create([Event("u001", ClockingEventType.In, new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero))],
            DirectoryFailureMode.ConnectionFailed).ExportMonthAsync(2026, 9);

        Assert.IsType<ExportResult.AbortedDirectoryUnavailable>(export); // UC-006 AF-2: abort, NO partial file
    }

    [Fact]
    public async Task ExportMonth_WritesColumnSetV1Header()
    {
        var export = await Create([Event("u001", ClockingEventType.In, new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero))]).ExportMonthAsync(2026, 9);
        var lines = Lines(Assert.IsType<ExportResult.Success>(export));
        Assert.Equal("ad_user_id,employee_name,department,office,event_timestamp,event_type", lines[0]);
    }

    [Fact]
    public async Task ExportMonth_MissingDisplayFields_WriteTrulyEmptyCells()
    {
        // UC-006 AF-3 — clause (d) at the CSV consumer: blank cells are TRULY EMPTY, never a
        // placeholder character, because the file reaches payroll.
        var export = await Create(
        [
            Event("u003", ClockingEventType.In, new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero)),
            Event("u999", ClockingEventType.Out, new DateTimeOffset(2026, 9, 2, 16, 30, 0, TimeSpan.Zero)),
        ]).ExportMonthAsync(2026, 9);

        var lines = Lines(Assert.IsType<ExportResult.Success>(export));
        Assert.Equal(3, lines.Length); // header + every event row — clause (a)
        Assert.Equal("u003,Ana Gomez,,,2026-09-01T08:58:12-04:00,in", lines[1]); // NOT "General", NOT "Central"
        Assert.Equal("u999,,,,2026-09-02T12:30:00-04:00,out", lines[2]); // unresolvable uid: ad_user_id still present
    }

    [Fact]
    public async Task ExportMonth_TimestampsAreIso8601WithDstAwareOffset()
    {
        var export = await Create(
        [
            Event("u001", ClockingEventType.In, new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero)),  // summer: -04:00
        ]).ExportMonthAsync(2026, 9);

        var lines = Lines(Assert.IsType<ExportResult.Success>(export));
        Assert.Equal("u001,Maria Gomez,Finance,Central,2026-09-01T08:58:12-04:00,in", lines[1]);
    }

    [Fact]
    public async Task ExportMonth_ValueWithComma_IsRfc4180Quoted()
    {
        var export = await Create([Event("u011", ClockingEventType.In, new DateTimeOffset(2026, 9, 3, 13, 15, 0, TimeSpan.Zero))]).ExportMonthAsync(2026, 9);

        var lines = Lines(Assert.IsType<ExportResult.Success>(export));
        Assert.Equal("u011,\"Gomez, Maria Clara\",Finance,South,2026-09-03T09:15:00-04:00,in", lines[1]);
    }

    [Fact]
    public async Task ExportMonth_MonthBoundsAreLocalCalendarDays()
    {
        // The payroll day is the LOCAL calendar day, never the server's: an event at 2026-09-01T03:30Z
        // is 2026-08-31 23:30 in America/Havana (August, EXCLUDED); 2026-09-01T04:30Z is 00:30 Sept 1 (INCLUDED).
        var export = await Create(
        [
            Event("u001", ClockingEventType.In, new DateTimeOffset(2026, 9, 1, 3, 30, 0, TimeSpan.Zero)),
            Event("u001", ClockingEventType.Out, new DateTimeOffset(2026, 9, 1, 4, 30, 0, TimeSpan.Zero)),
        ]).ExportMonthAsync(2026, 9);

        var lines = Lines(Assert.IsType<ExportResult.Success>(export));
        Assert.Single(lines, l => l.StartsWith("u001,")); // only the 00:30-local event belongs to September
        Assert.Contains("2026-09-01T00:30:00-04:00", lines[1]);
    }

    private sealed class FakeClockingsRepository(IReadOnlyList<ClockingEvent> events) : IClockingsRepository
    {
        public Task AddAsync(ClockingEvent @event) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClockingEvent>> GetByEmployeeAndRangeAsync(string employeeUid, DateTimeOffset fromUtc, DateTimeOffset toUtc) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClockingEvent>> GetByFilterAsync(ClockingFilter filter) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClockingEvent>> GetByRangeAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc)
            => Task.FromResult<IReadOnlyList<ClockingEvent>>(events.Where(e => e.RecordedAtUtc >= fromUtc && e.RecordedAtUtc < toUtc).ToList());
    }
}
