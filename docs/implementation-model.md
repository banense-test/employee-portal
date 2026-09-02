# Implementation Model — Employee Portal (Elaboration Baseline)

## Document Control

| Field | Value |
|---|---|
| Phase | Elaboration |
| Status | Draft — reverse-engineered from the integrated baseline on `main` at E5 close (2026-09-02). Authored by the Integrator from first-hand source reads (`scm_get_file_content`, SHAs cited per file); the mechanism code was implemented by the Implementer (PRs #3/#4/#5), reviewed by the Code Reviewer (APPROVED ×3), and integrated by the Integrator (PR #6 → `main`; PR #7/#8 at E4). |
| Milestone Target | End of Elaboration (LCA) — **NOT achieved; sanction withheld per the stakeholder's standing all-findings directive.** This model documents the IMPLEMENTED baseline the LCA evidence package cites: R001/R003/R004 mechanisms VERIFIED and RETIRED (Elaboration scope) on CI-traced evidence (formal TC pass 15 PASS · 0 FAIL · 8 BLOCKED — the 8 BLOCKED are a recorded SCOPE decision, deferred to Construction, not missing). |
| Materialization | Per Development Case §6.1: the Implementation Model is source code, not an upsertable document — it lives in SCM (this file), version-controlled, and merges through the review gate (E5-close PR #10). |

## Purpose and Method

This is the reverse-engineered Implementation Model of the integrated Elaboration baseline: what the code on `main` actually is, traced to the SAD components (COMP-001…011), Design Model classes (CLS-001…027) and interfaces (INT-006…019) it realizes. Method: every mechanism file was read first-hand from `main` via `scm_get_file_content` (SHAs cited below — the R6 gate verifies evidence by sha); signatures in the class view are the ACTUAL signatures, not the design's planned ones. The Elaboration prototype is EVOLUTIONARY production code (BRANCHING_STRATEGY §5.2) — nothing here is throwaway.

## Source Inventory (verified first-hand, main @ E5 close)

| Source file | SHA | Realizes | Risk |
|---|---|---|---|
| `src/EmployeePortal/Infrastructure/LdapGateway.cs` | b8df8b7 | COMP-007 / CLS-009 | R001 (HIGH) — RETIRED |
| `src/EmployeePortal/Infrastructure/KeycloakAuthProvider.cs` | 8758844 | COMP-006 / CLS-010 + OidcMiddleware | R003 — RETIRED |
| `src/EmployeePortal/Infrastructure/OfflineQueue.cs` | 58924df | COMP-009 / CLS-008 (server contract) | R004 — RETIRED |
| `src/EmployeePortal/Services/ClockingService.cs` | a16e2d7 | COMP-001 / CLS-001 | — |
| `src/EmployeePortal/Services/DirectoryService.cs` | aa76de4 | COMP-003 / CLS-003 | R001 consumer |
| `src/EmployeePortal/Services/ReportExport.cs` | 127c3d4 | COMP-010 / CLS-006 | R001 consumer |
| `src/EmployeePortal/Services/TimeConvention.cs` | 316faf3 | COMP-011 / CLS-007 | — |
| `src/EmployeePortal/Infrastructure/ClockingsRepository.cs` | (present) | COMP-008 INTERIM (in-memory) | F-CR-E3-1 — Construction Iter 1 |
| `src/EmployeePortal/wwwroot/js/offline-queue.js` | (present) | COMP-009 client half (localStorage) | R004 |
| `tests/EmployeePortal.Tests/**` + `Fixtures/` | (present) | dual-coverage suites + validation fixtures | — |

## Component View (reverse-engineered)

```plantuml
@startuml
skinparam componentStyle rectangle
skinparam fontSize 11
title Implementation Model — Reverse-Engineered Component View
Integrated Elaboration baseline on main (E5 close, 2026-09-02)

package "src/EmployeePortal — implemented (Elaboration baseline)" as IMPL {
  component "ClockingService\nCLS-001 · COMP-001\nServices/ClockingService.cs\nsha a16e2d7" as CLK
  component "DirectoryService\nCLS-003 · COMP-003\nServices/DirectoryService.cs\nsha aa76de4" as DIR
  component "ReportExportService\nCLS-006 · COMP-010\nServices/ReportExport.cs\nsha 127c3d4" as EXP
  component "TimeService\nCLS-007 · COMP-011\nServices/TimeConvention.cs\nsha 316faf3" as TIME
  component "KeycloakAuthProvider\nCLS-010 · COMP-006\nInfrastructure/KeycloakAuthProvider.cs\nsha 8758844" as AUTH
  component "LdapGateway\nCLS-009 · COMP-007\nInfrastructure/LdapGateway.cs\nsha b8df8b7" as LDAP
  component "OfflineQueue server contract\nCLS-008 · COMP-009\nInfrastructure/OfflineQueue.cs\nsha 58924df" as OFFQ
  component "InMemoryClockingsRepository\nINTERIM — F-CR-E3-1\nInfrastructure/ClockingsRepository.cs" as REPO
  component "OidcMiddleware\nrequest boundary (SEC-003)\nKeycloakAuthProvider.cs" as MW
}

package "wwwroot/js — client half" as CLIENT {
  component "offline-queue.js\nCOMP-009 client half\nlocalStorage queue (ADR-003)" as JSQ
}

package "Construction scope — NOT in the Elaboration tree" as FUTURE {
  component "NewsService\nCOMP-002" as NEWS <<deferred>>
  component "CategoryService\nCOMP-004" as CAT <<deferred>>
  component "AuditService\nCOMP-005" as AUD <<deferred>>
  component "PG Persistence\nCOMP-008 final INT-016 adapter" as PG <<deferred>>
}

DIR ..> LDAP : ILdapGateway\nsingle read path — the R001 bar\nholds in every consumer
EXP ..> DIR : IDirectoryService\nINT-008 transitive resolution
EXP ..> REPO : IClockingsRepository
EXP ..> TIME : ITimeConvention\nmonth bounds + ISO-8601 offset
CLK ..> REPO : IClockingsRepository\nUNIQUE idempotency_key (REL-002)
MW ..> AUTH : IAuthProvider
JSQ ..> CLK : idempotent sync endpoint\nreplay on reconnect (ADR-003)
OFFQ ..> JSQ : same contract —\nthe testable seam for the drop simulation

note bottom of LDAP
  R001 RETIRED: four-clause
  graceful degradation verified
  against the disposable LDAP
  directory (gaps seeded
  deliberately + substitution-
  attempt fixtures).
end note

note bottom of AUTH
  R003 RETIRED: token validation
  via JWKS, role claims (SEC-006),
  401 at the boundary — verified
  against the stub issuer.
  Round-trip state validation
  [DEFERRED — session mechanism,
  Construction].
end note

note bottom of REPO
  F-CR-E3-1 (carried): the final
  PostgreSQL adapter lands
  Construction Iteration 1
  per R008.
end note
@enduml
```

## Class View (reverse-engineered — actual signatures)

```plantuml
@startuml
skinparam classAttributeIconSize 0
skinparam fontSize 10
title Implementation Model — Reverse-Engineered Class View
Mechanism classes with actual signatures (main, E5 close)

package "Services" {
  interface IClockingService <<INT-006>>
  class ClockingService <<CLS-001>> {
    +GetCurrentStatusAsync(employeeUid) : Task<ClockingStatus>
    +RecordEventAsync(request) : Task<ClockingResult>
    +SyncEventsAsync(events) : Task<SyncResult>
    +GetHistoryAsync(employeeUid, fromUtc, toUtc) : Task<IReadOnlyList<ClockingEvent>>
    +GetClockingsAsync(filter) : Task<IReadOnlyList<ClockingEvent>>
  }
  interface IDirectoryService <<INT-008>>
  class DirectoryService <<CLS-003>> {
    +SearchAsync(criteria) : Task<DirectoryResult>
    +GetDisplayDataAsync(uids) : Task<IReadOnlyDictionary<string, EmployeeDisplayData>>
  }
  interface IReportExport <<INT-013>>
  class ReportExportService <<CLS-006>> {
    +ExportMonthAsync(year, month) : Task<ExportResult>
    -Csv(value) : string
  }
  interface ITimeConvention <<INT-014>>
  class TimeService <<CLS-007>> {
    +NowUtc() : DateTimeOffset
    +ToLocalDisplay(timestampUtc) : string
    +ToIso8601WithOffset(timestampUtc) : string
    +MonthBoundsLocal(year, month) : MonthBounds
  }
}

package "Infrastructure" {
  interface ILdapGateway
  class LdapGateway <<CLS-009>> {
    +SearchAsync(criteria) : Task<IReadOnlyList<DirectoryEntry>>
    +GetDisplayDataAsync(uids) : Task<IReadOnlyDictionary<string, EmployeeDisplayData>>
    -BuildFilter(criteria) : string
    -Get(raw, attribute) : string?
    -Escape(value) : string
  }
  interface IAuthProvider <<INT-011>>
  class KeycloakAuthProvider <<CLS-010>> {
    +ConfigureOidc(builder, options) : void
    +BuildAuthorizeRedirectUrl(redirectUri, state) : string
    +HandleOidcCallbackAsync(authorizationCode) : Task<AuthenticatedUser>
    +GetAuthenticatedUserAsync(context) : Task<AuthenticatedUser?>
    -ValidateTokenAsync(token) : Task<AuthenticatedUser>
    -MapRoles(payload) : IReadOnlySet<string>
  }
  class OidcMiddleware {
    +InvokeAsync(context) : Task
  }
  interface IOfflineQueue
  class InMemoryOfflineQueue <<CLS-008>> {
    +Enqueue(queued) : void
    +DequeueAll() : IReadOnlyList<QueuedClockingEvent>
    +Count : int
    {static} +Capacity : int
  }
  interface IClockingsRepository <<INT-016 interim>>
  class InMemoryClockingsRepository {
    INTERIM — F-CR-E3-1
  }
}

ClockingService ..|> IClockingService
ClockingService ..> IClockingsRepository
DirectoryService ..|> IDirectoryService
DirectoryService ..> ILdapGateway
LdapGateway ..|> ILdapGateway
ReportExportService ..|> IReportExport
ReportExportService ..> IClockingsRepository
ReportExportService ..> IDirectoryService
ReportExportService ..> ITimeConvention
TimeService ..|> ITimeConvention
KeycloakAuthProvider ..|> IAuthProvider
OidcMiddleware ..> IAuthProvider
InMemoryOfflineQueue ..|> IOfflineQueue
InMemoryClockingsRepository ..|> IClockingsRepository

note right of LdapGateway
  R001 four-clause bar in code:
  (a) every entry mapped — none dropped;
  (b) missing attribute never removes
  an entry; (c) never raises an error;
  (d) missing -> null, the FINAL value —
  never a default, placeholder, guessed
  value, or another employee's value.
  5 s hard timeout (PRF-003);
  RFC 4515 filter escaping.
end note

note right of ClockingService
  D-6 idempotent receiver: the UNIQUE
  idempotency_key constraint is the
  duplicate-suppression point
  (REL-002); an exact duplicate returns
  RejectedDuplicate — never a second
  row (ARCH-7). SAD reconciliation:
  no IAuditService call — NFR-005
  scopes audit to news + category.
end note

note bottom of TimeService
  Stakeholder-decided convention:
  store UTC (DAT-001); display
  America/Havana (IANA, DST-aware);
  export ISO-8601 with the offset in
  force at event time; payroll day =
  local calendar day. ARCH-4: no other
  class converts time.
end note
@enduml
```

## Test Coverage Map (dual coverage per mechanism)

```plantuml
@startuml
skinparam fontSize 10
title Implementation Model — Test Coverage Map
Dual-coverage suites (black-box contract + white-box paths) per mechanism

package "tests/EmployeePortal.Tests" {
  rectangle "BehaviouralBarTests" as T1
  rectangle "LdapGatewayTests" as T2
  rectangle "DirectoryServiceTests" as T3
  rectangle "KeycloakAuthProviderTests" as T4
  rectangle "OfflineQueueTests" as T5
  rectangle "OfflineResilienceTests" as T6
  rectangle "ClockingServiceTests" as T7
  rectangle "ReportExportServiceTests" as T8
  rectangle "TimeServiceTests" as T9
  rectangle "SmokeTests" as T10
  rectangle "Fixtures/DisposableLdapDirectory" as FX1
  rectangle "Fixtures/StubOidcIssuer" as FX2
}

rectangle "LdapGateway CLS-009 (R001)" as M1
rectangle "DirectoryService CLS-003" as M2
rectangle "KeycloakAuthProvider CLS-010 (R003)" as M3
rectangle "OfflineQueue CLS-008 (R004)" as M4
rectangle "ClockingService CLS-001" as M5
rectangle "ReportExportService CLS-006" as M6
rectangle "TimeService CLS-007" as M7

T1 ..> M1 : four-clause bar\nacross all four consumers
T1 ..> M2
T1 ..> M6
T2 ..> M1
T3 ..> M2
T4 ..> M3
T5 ..> M4
T6 ..> M4 : 5-minute drop\nsimulation (AC-005)
T7 ..> M5
T8 ..> M6
T9 ..> M7
FX1 ..> M1 : disposable directory\ngaps seeded deliberately\n+ substitution-attempt fixtures
FX2 ..> M3 : stub OIDC issuer\nno real Keycloak realm (CON-004)

note bottom of T1
  Formal TC-001..TC-023 pass:
  15 PASS / 0 FAIL / 8 BLOCKED.
  The 8 BLOCKED cases are a recorded
  SCOPE decision (production AD and
  Keycloak integration belongs to
  Construction) — deferred, not
  missing.
end note
@enduml
```

## Implementation Notes (verified in code, not in prose)

1. **R001 four-clause bar is IN the code** (`LdapGateway.Get`): missing/empty AD value → `null`, and null is the FINAL mapped value — never a default, placeholder, guessed value, or another employee's value. `GetDisplayDataAsync` guarantees map completeness (D-9): an unresolvable uid maps to an all-null `EmployeeDisplayData`, never omitted. `DirectoryService` re-guarantees completeness defensively. `ReportExportService.Csv(null)` renders a truly EMPTY cell — no placeholder character, no abort (UC-006 AF-3).
2. **R003 token validation** (`KeycloakAuthProvider.ValidateTokenAsync`): three-segment parse, RS256-only, kid-matched JWKS RSA verification, exp/iss/aud/sub checks, roles extracted from `realm_access.roles` VERBATIM (SEC-006); every rejection is `OidcTokenValidationException` → 401 at the boundary. Round-trip state validation carries the honest `[DEFERRED — lands with the session mechanism, Construction]` marker (F-CR-E3-3, closed at E4).
3. **R004 offline resilience**: `IOfflineQueue` is the server-side contract of COMP-009's client half (`offline-queue.js`, localStorage, ADR-003) — the testable seam for the drop simulation. Capacity ≥ 10 (REL-002); `DequeueAll` replays ordered by `RecordedAtUtc`; `ClockingService.SyncEventsAsync` persists each event through the idempotent receiver (D-6) — zero losses, zero duplicates (UNIQUE idempotency_key, ARCH-7).
4. **Time convention** (`TimeService`): `America/Havana` IANA zone (DST-aware — a hardcoded UTC-5 would silently shift payroll-day boundaries); month bounds computed as LOCAL calendar days converted to UTC; export ISO-8601 with the offset in force at event time. ARCH-4: no other class converts time.
5. **Interim persistence**: `InMemoryClockingsRepository` implements `IClockingsRepository` (INT-016 interim) — the final PostgreSQL adapter lands Construction Iteration 1 per R008 (F-CR-E3-1, carried with recorded owner). The interface seam means the swap touches no service code.

## Traceability

| Element | Traces From | Link Type | Traces To |
|---|---|---|---|
| `LdapGateway.cs` (CLS-009) | SAD COMP-007; Design Model CLS-009; R001; FR-010/CON-005; PR #3 (APPROVED) | Implements | R001 RETIRED (Elaboration); UC-004/005/006/007 behavioural bar; TC-011 + TC-021/022/023 |
| `KeycloakAuthProvider.cs` (CLS-010 + OidcMiddleware) | SAD COMP-006; Design Model CLS-010, INT-011; R003; CON-004; PR #4 (APPROVED) + PR #7/#8 (F-CR-E3-3) | Implements | R003 RETIRED (Elaboration); SEC-002/003/006; TC token-validation matrix |
| `OfflineQueue.cs` (CLS-008) + `offline-queue.js` | SAD COMP-009; Design Model CLS-008; R004; NFR-004/AC-005; ADR-003; PR #5 (APPROVED) | Implements | R004 RETIRED (Elaboration); REL-002/REL-003; drop simulation PASS |
| `ClockingService.cs` (CLS-001) | SAD COMP-001; Design Model CLS-001, INT-006; FR-004/FR-005; D-6/ARCH-7 | Implements | UC-001/UC-002; idempotent sync endpoint (ADR-003) |
| `DirectoryService.cs` (CLS-003) | SAD COMP-003; Design Model CLS-003, INT-008; FR-010; D-9 | Implements | UC-004; single read path for all four AD-reading consumers |
| `ReportExport.cs` (CLS-006) | SAD COMP-010; Design Model CLS-006, INT-013; FR-002; UC-006 AF-2/AF-3 | Implements | UC-006; CSV column set v1; ISO-8601 offset export |
| `TimeConvention.cs` (CLS-007) | SAD COMP-011; Design Model CLS-007, INT-014; stakeholder timestamp decision (Elab Iter 1) | Implements | DAT-001; USA-008; payroll-day boundaries |
| `ClockingsRepository.cs` (interim) | Design Model INT-016 (final contract); R008; F-CR-E3-1 | Implements | Construction Iteration 1 PG adapter (deferred, recorded disposition) |
| `tests/**` + `Fixtures/**` | Test Case TC-001…TC-023; R001/R003/R004 acceptance criteria | Tests | All mechanism classes above; formal pass 15/0/8 (CI run 33617748483) |
| Integrated baseline on `main` | PRs #3/#4/#5 → iteration/E1 (APPROVED ×3); PR #6 → main (APPROVED); PR #7 → iteration/E4; PR #8 → main (APPROVED) | Realizes | SAD 4+1 baseline; LCA evidence package; E5-close PR #10 (iteration/E5 → main) |
