using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using EmployeePortal.Tests.Fixtures;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>
/// The R001 FOUR-clause behavioural bar, clause by clause, across ALL FOUR AD-reading consumers
/// (UC-004 person card / UC-005 event row / UC-006 CSV row / UC-007 lookup) — the TC-011 +
/// TC-021/022/023 fixture shapes. Gaps AND substitution attempts are seeded deliberately in the
/// disposable directory so every clause can actually fail. The bar (stakeholder decisions,
/// Elab Iter 2 + verdict-gate contribution):
///   (a) every employee is rendered whether or not their attributes are complete;
///   (b) a missing attribute never removes someone from search results;
///   (c) a missing attribute never raises an error;
///   (d) a missing attribute is displayed as missing — never replaced by a default, a placeholder,
///       a guessed value, or another employee's value.
/// </summary>
public class BehaviouralBarTests
{
    private static DirectoryService CreateDirectory() => new(new LdapGateway(
        new DisposableLdapDirectory(),
        new LdapConnectionSettings("disposable-directory", 389)));

    private static ReportExportService CreateExport(IReadOnlyList<ClockingEvent> events)
        => new(new FakeClockingsRepository(events), CreateDirectory(), new TimeService());

    // ---- clause (a): every employee is rendered whether or not their attributes are complete ----

    [Fact]
    public async Task ClauseA_EveryEmployeeIsRendered_InAllFourConsumers()
    {
        var directory = CreateDirectory();

        // UC-004 person card: every matching employee is returned — complete AND gapped entries alike.
        var search = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Gomez"));
        Assert.Equal(4, search.Entries.Count); // Maria (complete), Luis (gapped), Ana (gapped), Maria Clara (complete)

        // UC-005 event row: every requested uid has a display entry — including one AD cannot resolve.
        var events = new List<ClockingEvent>
        {
            NewEvent("u003", ClockingEventType.In),   // Ana — gapped attributes
            NewEvent("u999", ClockingEventType.Out), // departed employee — AD cannot resolve the uid
        };
        var display = await directory.GetDisplayDataAsync(events.Select(e => e.EmployeeUid));
        Assert.Equal(2, display.Count);

        // UC-006 CSV row: every event row is written.
        var export = await CreateExport(events).ExportMonthAsync(2026, 9);
        var success = Assert.IsType<ExportResult.Success>(export);
        var lines = CsvLines(success);
        Assert.Equal(3, lines.Length); // header + 2 event rows — no row dropped

        // UC-007 lookup: the gapped employee is locatable by name.
        var lookup = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Ana Gomez"));
        Assert.Single(lookup.Entries);
    }

    // ---- clause (b): a missing attribute never removes someone from search results ----

    [Fact]
    public async Task ClauseB_MissingAttributeNeverRemovesSomeoneFromResults()
    {
        var directory = CreateDirectory();

        var gomez = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Gomez"));
        Assert.Contains(gomez.Entries, e => e.DisplayName == "Ana Gomez");    // missing department + office
        Assert.Contains(gomez.Entries, e => e.DisplayName == "Luis Gomez");    // missing job title + extension

        var diaz = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Diaz"));
        Assert.Contains(diaz.Entries, e => e.DisplayName == "Marco Diaz");     // ALL display attributes missing
    }

    // ---- clause (c): a missing attribute never raises an error ----

    [Fact]
    public async Task ClauseC_MissingAttributeNeverRaisesAnError_InAllFourConsumers()
    {
        var directory = CreateDirectory();

        var search = await Record.ExceptionAsync(() => directory.SearchAsync(new DirectorySearchCriteria(Name: "Diaz")));
        Assert.Null(search); // Marco (fully gapped) renders without error

        var display = await Record.ExceptionAsync(() => directory.GetDisplayDataAsync(["u008", "u999"]));
        Assert.Null(display);

        var export = await Record.ExceptionAsync(() => CreateExport(
        [
            NewEvent("u008", ClockingEventType.In),
            NewEvent("u999", ClockingEventType.Out),
        ]).ExportMonthAsync(2026, 9));
        Assert.Null(export); // fully-gapped + unresolvable employees export without abort

        var lookup = await Record.ExceptionAsync(() => directory.SearchAsync(new DirectorySearchCriteria(Name: "Marco Diaz")));
        Assert.Null(lookup);
    }

    // ---- clause (d): displayed as missing — never a default, placeholder, guessed value, or another employee's value ----

    [Fact]
    public async Task ClauseD_MissingAttributeDisplayedAsMissing_NeverSubstituted()
    {
        var directory = CreateDirectory();

        // UC-004 person card: the substitution-attempt fixtures must NOT be taken.
        var gomez = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Gomez"));
        var ana = gomez.Entries.Single(e => e.DisplayName == "Ana Gomez");
        Assert.Null(ana.Department); // NOT "General" — a default category is a fabrication
        Assert.Null(ana.Office);     // NOT "Central" — the first office in the list is a fabrication

        var perez = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Perez"));
        var john = perez.Single();
        Assert.Null(john.JobTitle);  // NOT "N/A" — a placeholder is a fabrication
        Assert.Null(john.Office);    // NOT "Central"

        // UC-005 event row: display fields are blank, never substituted.
        var display = await directory.GetDisplayDataAsync(["u003"]);
        var anaDisplay = display["u003"];
        Assert.Equal("Ana Gomez", anaDisplay.DisplayName);
        Assert.Null(anaDisplay.Department);
        Assert.Null(anaDisplay.Office);

        // UC-006 CSV row: missing display fields are TRULY EMPTY cells — no placeholder character,
        // because the file reaches payroll (stakeholder rationale, verbatim).
        var export = await CreateExport(
        [
            NewEvent("u003", ClockingEventType.In),
            NewEvent("u999", ClockingEventType.Out),
        ]).ExportMonthAsync(2026, 9);
        var success = Assert.IsType<ExportResult.Success>(export);
        var lines = CsvLines(success);
        Assert.Equal("u003,Ana Gomez,,,2026-09-01T08:58:12-04:00,in", lines[1]);
        Assert.Equal("u999,,,,2026-09-02T12:30:00-04:00,out", lines[2]);

        // UC-007 lookup: the gapped employee is locatable AND selectable (the uid is the selection
        // payload and is always present — CON-006) with blank fields.
        var lookup = await directory.SearchAsync(new DirectorySearchCriteria(Name: "Ana Gomez"));
        Assert.Single(lookup.Entries);
        var selectable = await directory.GetDisplayDataAsync(["u003"]);
        Assert.True(selectable.ContainsKey("u003"));

        // clause (d), cross-entry form: never ANOTHER EMPLOYEE'S value — the gapped entry does not
        // inherit anything from the complete entry rendered next to it.
        var luis = gomez.Entries.Single(e => e.DisplayName == "Luis Gomez");
        var maria = gomez.Entries.Single(e => e.DisplayName == "Maria Gomez");
        Assert.Null(luis.JobTitle);
        Assert.NotEqual(maria.JobTitle, luis.JobTitle);
        Assert.Null(luis.Extension);
        Assert.NotEqual(maria.Extension, luis.Extension);
    }

    // ---- helpers ----

    private static ClockingEvent NewEvent(string uid, ClockingEventType type) => new()
    {
        EmployeeUid = uid,
        EventType = type,
        RecordedAtUtc = type == ClockingEventType.In
            ? new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero)   // 08:58:12-04:00 local
            : new DateTimeOffset(2026, 9, 2, 16, 30, 0, TimeSpan.Zero),  // 12:30:00-04:00 local
        IdempotencyKey = $"key-{uid}-{type}",
    };

    private static string[] CsvLines(ExportResult.Success success)
        => System.Text.Encoding.UTF8.GetString(success.CsvBytes).Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

    private sealed class FakeClockingsRepository(IReadOnlyList<ClockingEvent> events) : IClockingsRepository
    {
        public Task AddAsync(ClockingEvent @event) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClockingEvent>> GetByEmployeeAndRangeAsync(string employeeUid, DateTimeOffset fromUtc, DateTimeOffset toUtc) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClockingEvent>> GetByFilterAsync(ClockingFilter filter) => throw new NotSupportedException();
        public Task<IReadOnlyList<ClockingEvent>> GetByRangeAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc)
            => Task.FromResult<IReadOnlyList<ClockingEvent>>(events.Where(e => e.RecordedAtUtc >= fromUtc && e.RecordedAtUtc < toUtc).ToList());
    }
}
