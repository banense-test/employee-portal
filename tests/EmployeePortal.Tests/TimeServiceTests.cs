using EmployeePortal.Services;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>CLS-007 TimeService — the stakeholder-decided timestamp convention (store UTC, display
/// America/Havana IANA DST-aware, export ISO-8601 with the offset in force at the event time,
/// payroll day = local calendar day). Cuba observes DST: a hardcoded UTC-5 would be wrong for part
/// of the year — these tests pin the DST-aware behaviour.</summary>
public class TimeServiceTests
{
    private readonly TimeService _time = new();

    [Fact]
    public void ToIso8601WithOffset_SummerEvent_CarriesDstOffset()
    {
        // 2026-09-01 is inside Cuba's DST window -> UTC-4
        var result = _time.ToIso8601WithOffset(new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero));
        Assert.Equal("2026-09-01T08:58:12-04:00", result);
    }

    [Fact]
    public void ToIso8601WithOffset_WinterEvent_CarriesStandardOffset()
    {
        // 2026-01-15 is outside Cuba's DST window -> UTC-5
        var result = _time.ToIso8601WithOffset(new DateTimeOffset(2026, 1, 15, 12, 58, 12, TimeSpan.Zero));
        Assert.Equal("2026-01-15T07:58:12-05:00", result);
    }

    [Fact]
    public void ToLocalDisplay_RendersHavanaLocalTime_NeverRawUtc()
    {
        var result = _time.ToLocalDisplay(new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero));
        Assert.Equal("2026-09-01 08:58:12", result); // USA-008: raw UTC or server time is never shown
    }

    [Fact]
    public void MonthBoundsLocal_September_AreLocalCalendarDays()
    {
        var bounds = _time.MonthBoundsLocal(2026, 9);
        Assert.Equal(new DateTimeOffset(2026, 9, 1, 4, 0, 0, TimeSpan.Zero), bounds.FromUtc);  // Sept 1 00:00 local (-04:00)
        Assert.Equal(new DateTimeOffset(2026, 10, 1, 4, 0, 0, TimeSpan.Zero), bounds.ToUtc);    // Oct 1 00:00 local (-04:00)
    }

    [Fact]
    public void MonthBoundsLocal_January_UsesStandardOffset()
    {
        var bounds = _time.MonthBoundsLocal(2026, 1);
        Assert.Equal(new DateTimeOffset(2026, 1, 1, 5, 0, 0, TimeSpan.Zero), bounds.FromUtc); // Jan 1 00:00 local (-05:00)
        Assert.Equal(new DateTimeOffset(2026, 2, 1, 5, 0, 0, TimeSpan.Zero), bounds.ToUtc);
    }

    [Fact]
    public void NowUtc_ReturnsUtcClock()
    {
        var before = DateTimeOffset.UtcNow;
        var now = _time.NowUtc();
        var after = DateTimeOffset.UtcNow;
        Assert.InRange(now, before, after);
        Assert.Equal(TimeSpan.Zero, now.Offset);
    }
}
