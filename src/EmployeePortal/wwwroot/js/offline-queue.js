// Employee Portal — offline queue (CLS-008, the browser half of COMP-009 — ADR-003).
// Contract: events queued in localStorage during a network drop (NFR-004 / AC-005), ordered by
// recordedAtUtc (REL-002), capacity >= 10 (REL-002); the confirmation renders from queued data
// < 1 s (PRF-002 offline path); on reconnect the queue replays via the idempotent sync endpoint
// (POST /api/clockings/sync) and clears on 200 OK (REL-003).
(function () {
    'use strict';

    var STORAGE_KEY = 'employeePortal.clockingQueue';
    var CAPACITY = 10; // REL-002

    function readQueue() {
        try {
            var raw = window.localStorage.getItem(STORAGE_KEY);
            return raw ? JSON.parse(raw) : [];
        } catch (error) {
            return []; // unreadable storage is treated as an empty queue — never a lost confirmation
        }
    }

    function writeQueue(events) {
        window.localStorage.setItem(STORAGE_KEY, JSON.stringify(events));
    }

    // Queues an event captured at the button press. The recorded timestamp and idempotency key
    // were captured at press time (DAT-001) and are NEVER rewritten — a queued event replays
    // with its original recorded timestamp.
    function enqueue(event) {
        var events = readQueue();
        if (events.length >= CAPACITY) {
            throw new Error('The offline queue is full (' + CAPACITY + ' events).');
        }
        events.push({ event: event, enqueuedAtUtc: new Date().toISOString() });
        writeQueue(events);
    }

    // PRF-002 offline path: the confirmation is rendered from the queued data — the user sees
    // their recorded time immediately, with the note that it will sync when the connection returns.
    function queuedConfirmation(event) {
        return {
            status: 'queued',
            recordedAtUtc: event.recordedAtUtc,
            eventType: event.eventType,
            message: 'Recorded. It will sync when the connection returns.'
        };
    }

    function queuedCount() {
        return readQueue().length;
    }

    // Replays the queue via the idempotent sync endpoint on reconnect. The server persists each
    // event or rejects it as an exact duplicate (UNIQUE idempotency_key — REL-002); the queue is
    // cleared only on a 200 OK (REL-003).
    function sync(fetchImpl) {
        var events = readQueue();
        if (events.length === 0) {
            return Promise.resolve({ persisted: 0, duplicatesRejected: 0 });
        }
        var payload = events.map(function (queued) { return queued.event; });
        return fetchImpl('/api/clockings/sync', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        }).then(function (response) {
            if (!response.ok) {
                throw new Error('Sync failed with status ' + response.status + ' — the queue is retained.');
            }
            writeQueue([]); // 200 OK: the queue is cleared (REL-003)
            return response.json();
        });
    }

    window.employeePortalOfflineQueue = {
        enqueue: enqueue,
        queuedConfirmation: queuedConfirmation,
        queuedCount: queuedCount,
        sync: sync,
        CAPACITY: CAPACITY,
        STORAGE_KEY: STORAGE_KEY
    };
})();
