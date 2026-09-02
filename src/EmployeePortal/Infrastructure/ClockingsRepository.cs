using EmployeePortal.Services;

namespace EmployeePortal.Infrastructure;

/// <summary>Thrown when an insert violates the UNIQUE idempotency_key contract (REL-002) — mirrors the PostgreSQL constraint violation that COMP-008 will enforce.</summary>
public sealed class DuplicateIdempotencyKeyException(string idempotencyKey)
    : Exception($"A clocking event with idempotency key '{idempotencyKey}' already exists.");

/// <summary>INT-016 IClockingsRepository — the persistence seam for clocking events (CLS-012).</summary>
public interface IClockingsRepository
{
    Task AddAsync(ClockingEvent @event);
    Task<IReadOnlyList<ClockingEvent>> GetByEmployeeAndRangeAsync(string employeeUid, DateTimeOffset fromUtc, DateTimeOffset toUtc);
    Task<IReadOnlyList<ClockingEvent>> GetByFilterAsync(ClockingFilter filter);
    Task<IReadOnlyList<ClockingEvent>> GetByRangeAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc);
}

/// <summary>
/// Interim in-memory adapter (Elaboration) — enforces the SAME UNIQUE idempotency_key contract (REL-002)
/// that the PostgreSQL constraint will enforce. Replaced by CLS-011/CLS-012 PgPersistence (EF Core +
/// Npgsql 10.0.3, ADR-002) in Construction Iteration 1 (R008 build-time validation).
/// ARCH-7: an exact duplicate is rejected, never duplicated.
/// </summary>
public sealed class InMemoryClockingsRepository : IClockingsRepository
{
    private readonly object _gate = new();
    private readonly List<ClockingEvent> _events = new();
    private int _nextId = 1;

    public Task AddAsync(ClockingEvent @event)
    {
        lock (_gate)
        {
            if (_events.Any(e => e.IdempotencyKey == @event.IdempotencyKey))
                throw new DuplicateIdempotencyKeyException(@event.IdempotencyKey);
            @event.Id = _nextId++;
            _events.Add(@event);
        }
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ClockingEvent>> GetByEmployeeAndRangeAsync(string employeeUid, DateTimeOffset fromUtc, DateTimeOffset toUtc)
    {
        lock (_gate)
        {
            return Task.FromResult<IReadOnlyList<ClockingEvent>>(_events
                .Where(e => e.EmployeeUid == employeeUid && e.RecordedAtUtc >= fromUtc && e.RecordedAtUtc < toUtc)
                .OrderBy(e => e.RecordedAtUtc)
                .ToList());
        }
    }

    public Task<IReadOnlyList<ClockingEvent>> GetByFilterAsync(ClockingFilter filter)
    {
        lock (_gate)
        {
            IEnumerable<ClockingEvent> query = _events;
            if (filter.EmployeeUid is not null) query = query.Where(e => e.EmployeeUid == filter.EmployeeUid);
            if (filter.FromUtc is not null) query = query.Where(e => e.RecordedAtUtc >= filter.FromUtc);
            if (filter.ToUtc is not null) query = query.Where(e => e.RecordedAtUtc < filter.ToUtc);
            return Task.FromResult<IReadOnlyList<ClockingEvent>>(query.OrderBy(e => e.RecordedAtUtc).ToList());
        }
    }

    public Task<IReadOnlyList<ClockingEvent>> GetByRangeAsync(DateTimeOffset fromUtc, DateTimeOffset toUtc)
    {
        lock (_gate)
        {
            return Task.FromResult<IReadOnlyList<ClockingEvent>>(_events
                .Where(e => e.RecordedAtUtc >= fromUtc && e.RecordedAtUtc < toUtc)
                .OrderBy(e => e.RecordedAtUtc)
                .ToList());
        }
    }
}
