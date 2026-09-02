using EmployeePortal.Infrastructure;
using EmployeePortal.Services;
using Xunit;

namespace EmployeePortal.Tests;

public class OfflineQueueTests
{
    private static QueuedClockingEvent Queued(string uid, ClockingEventType type, DateTimeOffset recordedAtUtc)
        => new(new ClockingEventDto(uid, type, recordedAtUtc, $"key-{uid}-{recordedAtUtc:O}"), recordedAtUtc);

    [Fact]
    public void Enqueue_DequeueAll_ReturnsEventsOrderedByRecordedTimestamp()
    {
        // REL-002: the queue is ordered by RecordedAtUtc, not arrival order — replay preserves
        // the employee's actual event sequence even when presses raced the network.
        var queue = new InMemoryOfflineQueue();
        var early = new DateTimeOffset(2026, 9, 1, 12, 0, 0, TimeSpan.Zero);
        var late = new DateTimeOffset(2026, 9, 1, 16, 0, 0, TimeSpan.Zero);
        queue.Enqueue(Queued("u001", ClockingEventType.Out, late));  // enqueued first, recorded later
        queue.Enqueue(Queued("u001", ClockingEventType.In, early));   // enqueued second, recorded earlier

        var dequeued = queue.DequeueAll();

        Assert.Equal(2, dequeued.Count);
        Assert.Equal(ClockingEventType.In, dequeued[0].Event.EventType);  // recorded order, not arrival order
        Assert.Equal(ClockingEventType.Out, dequeued[1].Event.EventType);
    }

    [Fact]
    public void DequeueAll_ClearsTheQueue()
    {
        var queue = new InMemoryOfflineQueue();
        queue.Enqueue(Queued("u001", ClockingEventType.In, DateTimeOffset.UtcNow));

        queue.DequeueAll();

        Assert.Equal(0, queue.Count);
        Assert.Empty(queue.DequeueAll());
    }

    [Fact]
    public void DequeueAll_EmptyQueue_ReturnsEmptyList()
    {
        var queue = new InMemoryOfflineQueue();
        Assert.Empty(queue.DequeueAll());
    }

    [Fact]
    public void Enqueue_BeyondCapacity_Throws_EventNotQueued()
    {
        var queue = new InMemoryOfflineQueue();
        for (var i = 0; i < InMemoryOfflineQueue.Capacity; i++) // REL-002: capacity >= 10
            queue.Enqueue(Queued("u001", ClockingEventType.In, DateTimeOffset.UtcNow.AddMinutes(i)));

        Assert.Throws<OfflineQueueFullException>(() =>
            queue.Enqueue(Queued("u001", ClockingEventType.In, DateTimeOffset.UtcNow.AddMinutes(99))));
        Assert.Equal(InMemoryOfflineQueue.Capacity, queue.Count); // the overflow event was NOT queued
    }

    [Fact]
    public void Capacity_IsAtLeastTen()
    {
        Assert.True(InMemoryOfflineQueue.Capacity >= 10, "REL-002 requires a queue capacity of at least 10 events.");
    }

    [Fact]
    public void Count_TracksEnqueuedEvents()
    {
        var queue = new InMemoryOfflineQueue();
        queue.Enqueue(Queued("u001", ClockingEventType.In, DateTimeOffset.UtcNow));
        queue.Enqueue(Queued("u001", ClockingEventType.Out, DateTimeOffset.UtcNow.AddHours(1)));
        Assert.Equal(2, queue.Count);
    }
}
