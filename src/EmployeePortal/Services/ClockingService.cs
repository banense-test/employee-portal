using EmployeePortal.Infrastructure;

namespace EmployeePortal.Services;

/// <summary>
/// INT-006 IClockingService (COMP-001) — clocking data model, time recording, idempotent endpoint.
/// </summary>
public interface IClockingService
{
    Task<ClockingStatus> GetCurrentStatusAsync(string employeeUid);
    Task<ClockingResult> RecordEventAsync(RecordClockingRequest request);
    Task<SyncResult> SyncEventsAsync(IEnumerable<ClockingEventDto> events);
    Task<IReadOnlyList<ClockingEvent>> GetHistoryAsync(string employeeUid, DateTimeOffset fromUtc, DateTimeOffset toUtc);
    Task<IReadOnlyList<ClockingEvent>> GetClockingsAsync(ClockingFilter filter);
}

/// <summary>
/// CLS-001 ClockingService (COMP-001). Design decision D-6 (idempotent receiver): the UNIQUE
/// idempotency key constraint — not application locking — is the duplicate-suppression point
/// (REL-002). SAD boundary reconciliation: CLS-001 does NOT call IAuditService — NFR-005 scopes
/// audit to news operations and category changes; clocking events carry their own actor and are
/// immutable (DAT-001).
/// </summary>
public sealed class ClockingService(IClockingsRepository clockings) : IClockingService
{
    public async Task<ClockingStatus> GetCurrentStatusAsync(string employeeUid)
    {
        if (string.IsNullOrWhiteSpace(employeeUid))
            throw new ArgumentException("The employee uid is required.", nameof(employeeUid));

        var events = await clockings.GetByEmployeeAndRangeAsync(employeeUid, DateTimeOffset.MinValue, DateTimeOffset.MaxValue);
        var last = events.LastOrDefault(); // ordered by RecordedAtUtc — the most recent persisted event
        return last is null || last.EventType == ClockingEventType.Out
            ? ClockingStatus.NotClockedIn
            : ClockingStatus.ClockedIn;
    }

    public async Task<ClockingResult> RecordEventAsync(RecordClockingRequest request)
    {
        Validate(request.EmployeeUid, request.IdempotencyKey);

        var @event = new ClockingEvent
        {
            EmployeeUid = request.EmployeeUid,
            EventType = request.EventType,
            RecordedAtUtc = request.RecordedAtUtc, // DAT-001: the press-time UTC capture — never rewritten
            IdempotencyKey = request.IdempotencyKey,
        };

        try
        {
            await clockings.AddAsync(@event);
        }
        catch (DuplicateIdempotencyKeyException)
        {
            return ClockingResult.RejectedDuplicate; // ARCH-7: an exact duplicate returns the original outcome, never a second row
        }

        return ClockingResult.Confirmed;
    }

    public async Task<SyncResult> SyncEventsAsync(IEnumerable<ClockingEventDto> events)
    {
        if (events is null) throw new ArgumentNullException(nameof(events));

        var persisted = 0;
        var duplicatesRejected = 0;
        foreach (var dto in events) // the queue replays ordered by RecordedAtUtc (REL-002)
        {
            Validate(dto.EmployeeUid, dto.IdempotencyKey);
            var result = await RecordEventAsync(new RecordClockingRequest(dto.EmployeeUid, dto.EventType, dto.RecordedAtUtc, dto.IdempotencyKey));
            if (result == ClockingResult.Confirmed) persisted++;
            else duplicatesRejected++;
        }

        return new SyncResult(persisted, duplicatesRejected); // zero losses: every event is persisted or reported as an exact duplicate
    }

    public Task<IReadOnlyList<ClockingEvent>> GetHistoryAsync(string employeeUid, DateTimeOffset fromUtc, DateTimeOffset toUtc)
    {
        if (string.IsNullOrWhiteSpace(employeeUid))
            throw new ArgumentException("The employee uid is required.", nameof(employeeUid));
        return clockings.GetByEmployeeAndRangeAsync(employeeUid, fromUtc, toUtc);
    }

    public Task<IReadOnlyList<ClockingEvent>> GetClockingsAsync(ClockingFilter filter)
        => clockings.GetByFilterAsync(filter ?? new ClockingFilter());

    private static void Validate(string employeeUid, string idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(employeeUid))
            throw new ArgumentException("The employee uid is required.", nameof(employeeUid));
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            throw new ArgumentException("The idempotency key is required (REL-002).", nameof(idempotencyKey));
    }
}
