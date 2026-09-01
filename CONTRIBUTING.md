# Contributing — Employee Portal

This file is the **programming-guidelines baseline** for every pull request. Code-review checklist item **CR-1** cites rules from here; a violation without a rule citation is personal taste, not a finding.

**Ownership:** architectural rules (ARCH-*) — Software Architect; coding conventions — Implementer; repository mechanics — ConfigurationManager. The authoritative branch strategy is `docs/BRANCHING_STRATEGY.md`.

## Architectural Rules (ARCH-1…ARCH-10 — Software Architect)

| Rule | Statement | Source |
|---|---|---|
| ARCH-1 | **Layering:** dependencies point DOWN only — `Pages/` → `Services/` → `Infrastructure/`. No upward or lateral concrete references. | SAD Implementation View |
| ARCH-2 | **Interfaces only:** every cross-package reference is an interface (`ICLK`, `INEWS`, `IDIR`, `ICAT`, `IEXPORT`, `IAUD`, `ILDAP`, `IPERSIST`, `ITIME`, `IAUTH`), never a concrete class. | SAD Logical View |
| ARCH-3 | **Composition root:** all wiring lives in `Program.cs` via .NET DI. No service locator; no manual construction of services in pages. | SAD Implementation View |
| ARCH-4 | **Timestamps:** stored in UTC (DAT-001); displayed via the Time Service in America/Havana (IANA, DST-aware); exported as ISO-8601 with explicit offset. Never render raw UTC or server time to a user. | SAD Timestamp Convention; COMP-011 |
| ARCH-5 | **Audit atomicity:** audit writes commit in the SAME database transaction as the state change they record (DAT-002). No audit row is ever updated or deleted. | SAD Process View; NFR-005 |
| ARCH-6 | **Graceful degradation:** a missing AD attribute renders as a blank field; the entry is NEVER hidden from results; a missing attribute NEVER raises an error. | R001 behavioural bar (stakeholder decision, Elab Iter 2); UC-004 AF-2 |
| ARCH-7 | **Idempotency:** clocking persistence uses `ON CONFLICT (idempotency_key) DO NOTHING` — an exact duplicate returns the original result, never a second row. | ADR-003; REL-002 |
| ARCH-8 | **Read-only LDAP:** the LDAP Gateway never writes to Active Directory. | CON-007 |
| ARCH-9 | **No employee data in the portal database** beyond `worker_categories` (`employee_uid` + `category`). Directory data is always read live from AD. | CON-006 |
| ARCH-10 | **Evolutionary mechanisms:** PoC mechanism code lives in `src/` as production code — never in a `poc/` branch or `samples/` directory. | BRANCHING_STRATEGY §8.4 |

## Coding Conventions (Implementer)

- **Naming:** PascalCase for public types and members; camelCase for locals and parameters; async methods suffixed `Async`.
- **Error handling:** catch specific exceptions, never a bare `catch`; user-facing failures use the designated error pages; no swallowed exceptions.
- **Async:** `async`/`await` end-to-end for all I/O (LDAP, PostgreSQL, HTTP); no `.Result`/`.Wait()` (deadlock risk).
- **Tests:** dual coverage per mechanism — black-box contract AND white-box paths (branches, loops, error handlers); test names follow `Given_When_Then`.

## Branch Strategy

Authoritative document: `docs/BRANCHING_STRATEGY.md`. Essentials:

- Mechanism branches: `feature/E1-{risk-id}` created from `iteration/E1`.
- Label the branch `ready-for-review` when handed off; the Code Reviewer opens one PR per branch (base `iteration/E1`).
- Only the Integrator writes `iteration/*` branches.
- PR body carries a traceability trailer: `Implements: UC-NNN` or the risk id.

## PR Checklist

- CI green (hard gate — a red build is request_changes, no code review)
- Dual-coverage tests included for every mechanism
- Traceability trailer in the PR body
- Changed files under `src/` or `tests/` only (build-tree coverage)
- No violations of ARCH-1…ARCH-10 (cite the rule number in review comments)
