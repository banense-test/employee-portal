using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>White-box coverage for CLS-001 ClockingService — the idempotent receiver (D-6), the
/// status rule, and the query paths.</summary>
public class ClockingServiceTests
{
    private static ClockingService Create(out InMemoryClockingsRepository repository)
    {
        repository = new InMemoryClockingsRepository();
        return new ClockingService(repository);
    }

    private static RecordClockingRequest Request(string uid = "u001", ClockingEventType type = ClockingEventType.In,
        DateTimeOffset? recordedAtUtc = null, string? key = null)
        => new(uid, type, recordedAtUtc ?? new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero), key ?? $"key-{uid}-{type}-{Guid.NewGuid():N}");

    [Fact]
    public async Task RecordEvent_PersistsExactlyOneEvent()
    {
        var service = Create(out var repository);

        var result = await service.RecordEventAsync(Request());

        Assert.Equal(ClockingResult.Confirmed, result);
        Assert.Single(await repository.GetByFilterAsync(new ClockingFilter(EmployeeUid: "u001")));
    }

    [Fact]
    public async Task RecordEvent_ExactDuplicate_ReturnsRejectedDuplicate_NeverASecondRow()
    {
        var service = Create(out var repository);
        var request = Request(key: "the-same-key");

        await service.RecordEventAsync(request);
        var second = await service.RecordEventAsync(request);

        Assert.Equal(ClockingResult.RejectedDuplicate, second); // REL-002 / ARCH-7
        Assert.Single(await repository.GetByFilterAsync(new ClockingFilter(EmployeeUid: "u001")));
    }

    [Fact]
    public async Task RecordEvent_EmptyEmployeeUid_Throws()
    {
        var service = Create(out _);
        await Assert.ThrowsAsync<ArgumentException>(() => service.RecordEventAsync(Request(uid: "")));
    }

    [Fact]
    public async Task RecordEvent_EmptyIdempotencyKey_Throws()
    {
        var service = Create(out _);
        await Assert.ThrowsAsync<ArgumentException>(() => service.RecordEventAsync(Request(key: "")));
    }

    [Fact]
    public async Task SyncEvents_MixedBatch_ReportsPersistedAndDuplicatesRejected()
    {
        var service = Create(out var repository);
        await service.RecordEventAsync(Request(key: "already-persisted")); // pre-existing event

        var batch = new List<ClockingEventDto>
        {
            new("u001", ClockingEventType.In, new DateTimeOffset(2026, 9, 1, 13, 0, 0, TimeSpan.Zero), "already-persisted"), // exact duplicate
            new("u001", ClockingEventType.Out, new DateTimeOffset(2026, 9, 1, 17, 0, 0, TimeSpan.Zero), "new-event-1"),
            new("u002", ClockingEventType.In, new DateTimeOffset(2026, 9, 2, 12, 0, 0, TimeSpan.Zero), "new-event-2"),
        };

        var result = await service.SyncEventsAsync(batch);

        Assert.Equal(2, result.Persisted);
        Assert.Equal(1, result.DuplicatesRejected);
        Assert.Equal(3, (await repository.GetByFilterAsync(new ClockingFilter())).Count); // 1 pre-existing + 2 new — zero losses, zero duplicates
    }

    [Fact]
    public async Task SyncEvents_EmptyBatch_ReportsZeroZero()
    {
        var service = Create(out _);
        var result = await service.SyncEventsAsync([]);
        Assert.Equal(new SyncResult(0, 0), result);
    }

    [Fact]
    public async Task SyncEvents_NullBatch_Throws()
    {
        var service = Create(out _);
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.SyncEventsAsync(null!));
    }

    [Fact]
    public async Task SyncEvents_InvalidItem_Throws()
    {
        var service = Create(out _);
        var batch = new List<ClockingEventDto> { new("", ClockingEventType.In, DateTimeOffset.UtcNow, "key") };
        await Assert.ThrowsAsync<ArgumentException>(() => service.SyncEventsAsync(batch));
    }

    [Fact]
    public async Task GetCurrentStatus_NoEvents_NotClockedIn()
    {
        var service = Create(out _);
        Assert.Equal(ClockingStatus.NotClockedIn, await service.GetCurrentStatusAsync("u001"));
    }

    [Fact]
    public async Task GetCurrentStatus_LastEventIn_ClockedIn()
    {
        var service = Create(out _);
        await service.RecordEventAsync(Request(type: ClockingEventType.In));
        Assert.Equal(ClockingStatus.ClockedIn, await service.GetCurrentStatusAsync("u001"));
    }

    [Fact]
    public async Task GetCurrentStatus_LastEventOut_NotClockedIn()
    {
        var service = Create(out _);
        await service.RecordEventAsync(Request(type: ClockingEventType.In, key: "k1"));
        await service.RecordEventAsync(Request(type: ClockingEventType.Out, key: "k2"));
        Assert.Equal(ClockingStatus.NotClockedIn, await service.GetCurrentStatusAsync("u001"));
    }

    [Fact]
    public async Task GetCurrentStatus_EmptyUid_Throws()
    {
        var service = Create(out _);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetCurrentStatusAsync(""));
    }

    [Fact]
    public async Task GetHistory_ReturnsOnlyOwnEventsInRange()
    {
        var service = Create(out _);
        await service.RecordEventAsync(Request(uid: "u001", key: "k1",
            recordedAtUtc: new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero)));
        await service.RecordEventAsync(Request(uid: "u001", key: "k2",
            recordedAtUtc: new DateTimeOffset(2026, 10, 1, 12, 0, 0, TimeSpan.Zero))); // outside September
        await service.RecordEventAsync(Request(uid: "u002", key: "k3")); // another employee

        var history = await service.GetHistoryAsync("u001",
            new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero));

        Assert.Single(history); // SEC-007 shape: only the employee's own events in range
    }

    [Fact]
    public async Task GetHistory_EmptyUid_Throws()
    {
        var service = Create(out _);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GetHistoryAsync("",
            DateTimeOffset.MinValue, DateTimeOffset.MaxValue));
    }

    [Fact]
    public async Task GetClockings_FilterByEmployeeAndRange()
    {
        var service = Create(out _);
        await service.RecordEventAsync(Request(uid: "u001", key: "k1",
            recordedAtUtc: new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero)));
        await service.RecordEventAsync(Request(uid: "u002", key: "k2",
            recordedAtUtc: new DateTimeOffset(2026, 9, 2, 12, 0, 0, TimeSpan.Zero)));
        await service.RecordEventAsync(Request(uid: "u001", key: "k3",
            recordedAtUtc: new DateTimeOffset(2026, 8, 1, 12, 0, 0, TimeSpan.Zero))); // outside range

        var filter = new ClockingFilter(
            EmployeeUid: "u001",
            FromUtc: new DateTimeOffset(2026, 9, 1, 0, 0, 0, TimeSpan.Zero),
            ToUtc: new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero));
        var events = await service.GetClockingsAsync(filter);

        Assert.Single(events);
    }

    [Fact]
    public async Task GetClockings_NullFilter_ReturnsAll()
    {
        var service = Create(out _);
        await service.RecordEventAsync(Request(uid: "u001", key: "k1"));
        await service.RecordEventAsync(Request(uid: "u002", key: "k2"));

        var events = await service.GetClockingsAsync(null!);

        Assert.Equal(2, events.Count);
    }
}
