using System.Globalization;

namespace EmployeePortal.Services;

/// <summary>
/// INT-014 ITimeConvention — the single owner of the timestamp convention (CLS-007, COMP-011).
/// ARCH-4: no other class converts time.
/// </summary>
public interface ITimeConvention
{
    DateTimeOffset NowUtc();
    string ToLocalDisplay(DateTimeOffset timestampUtc);
    string ToIso8601WithOffset(DateTimeOffset timestampUtc);
    MonthBounds MonthBoundsLocal(int year, int month);
}

/// <summary>UTC bounds of a local calendar month — the payroll day is the local calendar day, never the server's.</summary>
public sealed record MonthBounds(DateTimeOffset FromUtc, DateTimeOffset ToUtc);

/// <summary>
/// CLS-007 TimeService (COMP-011) — the stakeholder-decided convention (Elab Iter 1):
/// store UTC (DAT-001); display America/Havana (IANA identifier, DST-aware — USA-008; a hardcoded
/// UTC-5 would be wrong for part of the year); export ISO-8601 with the offset in force at the event
/// time per the IANA zone database; payroll day = the local calendar day.
/// </summary>
public sealed class TimeService : ITimeConvention
{
    private const string OfficeTimeZoneId = "America/Havana";

    public DateTimeOffset NowUtc() => DateTimeOffset.UtcNow;

    public string ToLocalDisplay(DateTimeOffset timestampUtc)
    {
        var local = TimeZoneInfo.ConvertTime(timestampUtc, Zone);
        return local.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public string ToIso8601WithOffset(DateTimeOffset timestampUtc)
    {
        var local = TimeZoneInfo.ConvertTime(timestampUtc, Zone);
        return local.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
    }

    public MonthBounds MonthBoundsLocal(int year, int month)
    {
        var startLocal = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var nextYear = month == 12 ? year + 1 : year;
        var nextMonth = month == 12 ? 1 : month + 1;
        var endLocal = new DateTime(nextYear, nextMonth, 1, 0, 0, 0, DateTimeKind.Unspecified);
        return new MonthBounds(ToUtc(startLocal), ToUtc(endLocal));
    }

    private static DateTimeOffset ToUtc(DateTime localWallTime)
    {
        var utc = TimeZoneInfo.ConvertTimeToUtc(localWallTime, Zone);
        return new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
    }

    private static readonly TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(OfficeTimeZoneId);
}
