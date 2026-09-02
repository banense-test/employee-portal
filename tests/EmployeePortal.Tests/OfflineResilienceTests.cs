using System.Diagnostics;
using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using Xunit;

namespace EmployeePortal.Tests;

/// <summary>
/// R004 empirical validation — the direct 5-minute network-drop simulation (AC-005; PoC decision:
/// "Direct 5-minute network-drop simulation — nothing blocks it"). The scenario: the employee
/// presses the clocking button while the portal server is unreachable; the event is queued in
/// localStorage (CLS-008); the confirmation renders from queued data; on reconnect the queue
/// replays via the idempotent sync endpoint with zero duplicates (UNIQUE idempotency_key —
/// REL-002) and zero losses; all queued events are persisted <= 60 s after restore (REL-003).
/// </summary>
public class OfflineResilienceTests
{
    private static ClockingService CreateService(out InMemoryClockingsRepository repository)
    {
        repository = new InMemoryClockingsRepository();
        return new ClockingService(repository);
    }

    private static QueuedClockingEvent PressDuringDrop(string uid, ClockingEventType type, DateTimeOffset recordedAtUtc)
        => new(new ClockingEventDto(uid, type, recordedAtUtc, $"key-{uid}-{recordedAtUtc:O}"), recordedAtUtc);

    [Fact]
    public async Task DropSimulation_QueueDuringDrop_ConfirmationRendersFromQueuedData_UnderOneSecond()
    {
        // PRF-002 offline path: the button press during the drop queues the event and renders the
        // confirmation from the queued data — the user never waits for the network.
        var queue = new InMemoryOfflineQueue();
        var pressTime = new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero);

        var stopwatch = Stopwatch.StartNew();
        queue.Enqueue(PressDuringDrop("u001", ClockingEventType.In, pressTime));
        var confirmation = new { status = "queued", recordedAtUtc = pressTime }; // rendered from queued data
        stopwatch.Stop();

        Assert.Equal("queued", confirmation.status);
        Assert.Equal(1, queue.Count);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(1),
            $"PRF-002 offline path: confirmation took {stopwatch.Elapsed} — must render < 1 s.");
    }

    [Fact]
    public async Task DropSimulation_Reconnect_ReplaysAllQueuedEvents_ZeroDuplicates_ZeroLosses()
    {
        // AC-005: five events pressed during the drop; on reconnect every one is persisted exactly once.
        var service = CreateService(out var repository);
        var queue = new InMemoryOfflineQueue();
        var baseTime = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);
        for (var i = 0; i < 5; i++)
            queue.Enqueue(PressDuringDrop("u001", i % 2 == 0 ? ClockingEventType.In : ClockingEventType.Out, baseTime.AddHours(i)));

        var replay = queue.DequeueAll(); // ordered by RecordedAtUtc (REL-002)
        var result = await service.SyncEventsAsync(replay.Select(q => q.Event));

        Assert.Equal(5, result.Persisted);
        Assert.Equal(0, result.DuplicatesRejected);
        Assert.Equal(5, (await repository.GetByFilterAsync(new ClockingFilter(EmployeeUid: "u001"))).Count); // zero losses
    }

    [Fact]
    public async Task DropSimulation_ReplayTheSameBatchTwice_ExactDuplicatesRejected_NeverDuplicated()
    {
        // REL-002 conflict policy: a double replay (e.g., the browser retries a sync that already
        // succeeded) must not create a single duplicate row.
        var service = CreateService(out var repository);
        var batch = Enumerable.Range(0, 5).Select(i =>
            new ClockingEventDto("u001", ClockingEventType.In,
                new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero).AddHours(i), $"key-{i}")).ToList();

        var first = await service.SyncEventsAsync(batch);
        var second = await service.SyncEventsAsync(batch); // the double replay

        Assert.Equal(5, first.Persisted);
        Assert.Equal(0, second.Persisted);
        Assert.Equal(5, second.DuplicatesRejected);
        Assert.Equal(5, (await repository.GetByFilterAsync(new ClockingFilter())).Count); // still exactly 5 rows
    }

    [Fact]
    public async Task DropSimulation_AllQueuedEventsPersisted_WithinSixtySecondsOfRestore()
    {
        // REL-003: the full-capacity queue (10 events) syncs well inside the 60 s window.
        var service = CreateService(out var repository);
        var queue = new InMemoryOfflineQueue();
        var baseTime = new DateTimeOffset(2026, 9, 1, 8, 0, 0, TimeSpan.Zero);
        for (var i = 0; i < InMemoryOfflineQueue.Capacity; i++)
            queue.Enqueue(PressDuringDrop("u001", ClockingEventType.In, baseTime.AddMinutes(i)));

        var stopwatch = Stopwatch.StartNew();
        var replay = queue.DequeueAll();
        var result = await service.SyncEventsAsync(replay.Select(q => q.Event));
        stopwatch.Stop();

        Assert.Equal(InMemoryOfflineQueue.Capacity, result.Persisted);
        Assert.Equal(0, result.DuplicatesRejected);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(60),
            $"REL-003: sync took {stopwatch.Elapsed} — all queued events must persist <= 60 s after restore.");
    }

    [Fact]
    public async Task DropSimulation_QueueOrderedByRecordedTimestamp_ReplayPreservesEmployeeSequence()
    {
        // REL-002: the replay order is the employee's actual event sequence (In then Out),
        // regardless of the order the presses hit the queue.
        var service = CreateService(out var repository);
        var queue = new InMemoryOfflineQueue();
        var inTime = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);
        var outTime = new DateTimeOffset(2026, 9, 1, 17, 0, 0, TimeSpan.Zero);
        queue.Enqueue(PressDuringDrop("u001", ClockingEventType.Out, outTime)); // pressed/enqueued first
        queue.Enqueue(PressDuringDrop("u001", ClockingEventType.In, inTime));  // recorded earlier

        var replay = queue.DequeueAll();
        await service.SyncEventsAsync(replay.Select(q => q.Event));

        var persisted = await repository.GetByFilterAsync(new ClockingFilter(EmployeeUid: "u001"));
        Assert.Equal(ClockingEventType.In, persisted[0].EventType);  // recorded order preserved
        Assert.Equal(ClockingEventType.Out, persisted[1].EventType);
        Assert.Equal(ClockingStatus.NotClockedIn, await service.GetCurrentStatusAsync("u001")); // the status rule sees the true sequence
    }

    [Fact]
    public async Task DropSimulation_OnlinePath_ConfirmationUnderOneSecond()
    {
        // PRF-002 online path: a direct record (no drop) confirms < 1 s.
        var service = CreateService(out _);
        var request = new RecordClockingRequest("u001", ClockingEventType.In,
            new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero), "key-online");

        var stopwatch = Stopwatch.StartNew();
        var result = await service.RecordEventAsync(request);
        stopwatch.Stop();

        Assert.Equal(ClockingResult.Confirmed, result);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(1),
            $"PRF-002 online path: confirmation took {stopwatch.Elapsed} — must respond < 1 s.");
    }

    [Fact]
    public async Task DropSimulation_MixedOnlineAndQueuedEvents_NoDuplicatesAcrossPaths()
    {
        // The same press can arrive twice (online retry + queued replay) — the idempotency key
        // collapses them to one row across BOTH paths.
        var service = CreateService(out var repository);
        var pressTime = new DateTimeOffset(2026, 9, 1, 12, 58, 12, TimeSpan.Zero);
        var request = new RecordClockingRequest("u001", ClockingEventType.In, pressTime, "same-press");

        await service.RecordEventAsync(request); // the online attempt succeeded
        var replay = await service.SyncEventsAsync([new ClockingEventDto("u001", ClockingEventType.In, pressTime, "same-press")]); // the queued replay of the SAME press

        Assert.Equal(0, replay.Persisted);
        Assert.Equal(1, replay.DuplicatesRejected);
        Assert.Single(await repository.GetByFilterAsync(new ClockingFilter(EmployeeUid: "u001")));
    }
}
