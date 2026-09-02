using EmployeePortal.Services;

namespace EmployeePortal.Infrastructure;

/// <summary>Thrown when the offline queue is at capacity (REL-002: capacity >= 10 events).</summary>
public sealed class OfflineQueueFullException(int capacity)
    : Exception($"The offline queue is full ({capacity} events). The event was NOT queued.");

/// <summary>
/// CLS-008 OfflineQueue — the server-side contract of COMP-009's client half (ADR-003).
/// The browser realization (localStorage) implements the same contract in
/// wwwroot/js/offline-queue.js; this interface is the testable seam for the drop simulation.
/// </summary>
public interface IOfflineQueue
{
    /// <summary>Queues an event captured during a network drop. Throws OfflineQueueFullException at capacity.</summary>
    void Enqueue(QueuedClockingEvent queued);

    /// <summary>Removes and returns ALL queued events, ordered by RecordedAtUtc (REL-002 — replay preserves the employee's actual event sequence).</summary>
    IReadOnlyList<QueuedClockingEvent> DequeueAll();

    int Count { get; }
}

/// <summary>In-memory realization of the offline queue contract (capacity >= 10 — REL-002).</summary>
public sealed class InMemoryOfflineQueue : IOfflineQueue
{
    public const int Capacity = 10;

    private readonly object _gate = new();
    private readonly List<QueuedClockingEvent> _events = new();

    public int Count
    {
        get { lock (_gate) return _events.Count; }
    }

    public void Enqueue(QueuedClockingEvent queued)
    {
        lock (_gate)
        {
            if (_events.Count >= Capacity)
                throw new OfflineQueueFullException(Capacity);
            _events.Add(queued);
        }
    }

    public IReadOnlyList<QueuedClockingEvent> DequeueAll()
    {
        lock (_gate)
        {
            var ordered = _events
                .OrderBy(q => q.Event.RecordedAtUtc)
                .ThenBy(q => q.EnqueuedAtUtc)
                .ToList();
            _events.Clear();
            return ordered;
        }
    }
}
