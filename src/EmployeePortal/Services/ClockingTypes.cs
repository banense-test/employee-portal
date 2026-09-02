namespace EmployeePortal.Services;

public enum ClockingEventType { In, Out }

public enum ClockingStatus { ClockedIn, NotClockedIn }

public enum ClockingResult { Confirmed, RejectedDuplicate }

/// <summary>
/// CLS-021 ClockingEvent — immutable after capture (DAT-001: no update path exists);
/// IdempotencyKey carries the UNIQUE contract (REL-002). SyncedAtUtc is null for a direct
/// online insert and set when the event arrived via offline sync replay (ADR-003).
/// </summary>
public sealed class ClockingEvent
{
    public int Id { get; set; }
    public string EmployeeUid { get; set; } = string.Empty;
    public ClockingEventType EventType { get; set; }
    public DateTimeOffset RecordedAtUtc { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public DateTimeOffset? SyncedAtUtc { get; set; }
}

/// <summary>Online record request — RecordedAtUtc is the press-time UTC capture (DAT-001).</summary>
public sealed record RecordClockingRequest(string EmployeeUid, ClockingEventType EventType, DateTimeOffset RecordedAtUtc, string IdempotencyKey);

/// <summary>Sync endpoint payload item (SEQ-001 AF-1) — a queued event replays with its original recorded timestamp unchanged.</summary>
public sealed record ClockingEventDto(string EmployeeUid, ClockingEventType EventType, DateTimeOffset RecordedAtUtc, string IdempotencyKey);

/// <summary>CLS-008 queue item — ClockingEventDto + the moment it was enqueued.</summary>
public sealed record QueuedClockingEvent(ClockingEventDto Event, DateTimeOffset EnqueuedAtUtc);

/// <summary>UC-005 HR review filter — null components are unbounded.</summary>
public sealed record ClockingFilter(string? EmployeeUid = null, DateTimeOffset? FromUtc = null, DateTimeOffset? ToUtc = null);

/// <summary>Sync outcome: every event either persisted or rejected as an exact duplicate — zero losses, zero duplicates (REL-002).</summary>
public sealed record SyncResult(int Persisted, int DuplicatesRejected);
