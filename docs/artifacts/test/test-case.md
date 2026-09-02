## Document Control
| Field | Value |
|---|---|
| Phase | Elaboration |
| Status | Draft — 23 test cases DESIGNED. **Iter 3 (this revision): A-28 EXECUTED — the FOURTH behavioural-bar clause (stakeholder contribution at the Iter 2 verdict gate, binding) is propagated to TC-011 + TC-021/022/023 with fourth-clause verification steps (assert the rendered/exported value is EXACTLY blank — never a default, a placeholder, a guessed value, or another employee's value) and substitution-attempt fixtures seeded in the disposable LDAP directory (a "General" default-department temptation, a "Central" first-office fallback, an "N/A" placeholder title, a fully-gapped entry) so clause (d) can actually fail. Execution-state transition recorded honestly: the three mechanisms are MERGED to `iteration/E1` (verified first-hand this revision — LdapGateway.cs sha b8df8b7, KeycloakAuthProvider.cs sha 7bd4cfd, ClockingsRepository.cs sha 017cbcd, offline-queue.js sha 9ac644a, EmployeePortal.Tests.csproj sha 23b9d1 with xunit 2.9.2 + Microsoft.NET.Test.Sdk 17.12.0); the 19 mechanism-covered cases transition Designed → Scripted with verdicts **PENDING — none claimed** (the formal execution pass and the PoC results ledger own the verdicts); TC-013…TC-016 remain BLOCKED on Construction scheduling (news/audit is Construction scope — never an Issue #1 blocker). No pass counts, fail counts, or durations claimed or fabricated |
| Milestone Target | End of Elaboration (LCA) — NOT yet achieved |
| Iteration | 3 (Cycle 1) — convergence cycle continuation |
| Date | 2026-09-02 |
| Elaboration Changes | **Iter 3 (this revision):** (1) **A-28 executed (Test Designer-owned action from the Review Record's stakeholder-contribution propagation chain, deadline BEFORE TC execution — met):** TC-011 and TC-021/022/023 extended with fourth-clause verification steps; the disposable LDAP fixture re-seeded with substitution-attempt fixtures per the stakeholder's direction ("Add a fourth clause to all four"); catalog, goals table, automation architecture, workflow/lifecycle diagrams, test data and traceability updated — the R001 bar is now FOUR clauses × four consumers. (2) **Execution-state transition (honest, evidence-based):** the Cycle 1–2 blocking cause (Issue #1 — mechanism code absent) is resolved at the source: the Implementer handed off three ready-for-review branches, the Code Reviewer issued 3 APPROVED terminal dispositions (PRs #3/#4/#5 base `iteration/E1`, CI green ×3, per the Review Record Iter 3 code-review-lens record), and the mechanisms are merged to `iteration/E1` — verified first-hand this revision via file probes (shas above); the 19 mechanism-covered cases are Scripted (Implementer dual-coverage suites, CR-2 verified) with verdicts PENDING; TC-013…TC-016 remain BLOCKED on Construction scheduling. **Iter 2 (preserved):** Risk List F1 share resolved — TC-011 rewritten to the R001 behavioural bar (the >90% statistical criterion dropped per the stakeholder decision: invented, and against self-seeded data it cannot fail, so it proves nothing); TC-021/022/023 designed (four-consumer coverage, stakeholder-confirmed "Yes"); Cycle 2 execution record: all 23 BLOCKED with branch-level evidence (zero CI runs on the mechanism branches — the handoff absent at the source). **Iter 1 (preserved):** 20 test cases (TC-001…TC-020) covering UC-001, UC-004, UC-010 (test priority 1), the R003 authentication mechanism, and SEC-006/SEC-007 — each with preconditions, input data, expected outcome, pass/fail criteria, attacked failure scenario, automation hints, and interface points (INT-006…INT-019); automation architecture specified; Cycle 1 execution record preserved in § Findings |
## Test Scope
### Evaluation Mission Alignment

This test model is the verification counterpart of the Use-Case Model for the **Elaboration Evaluation Mission** (Test Evaluation Summary): empirically validate the three architecturally significant mechanisms — **R001 (HIGH, disposable LDAP directory) > R003 (SIGNIFICANT, stub OIDC issuer) > R004 (SIGNIFICANT, direct)** — against the architecture baseline (SAD COMP-001…011, ADR-001…004; Design Model CLS-001…027, INT-006…INT-019). Per the binding stakeholder decision, the PoC is produced in Elaboration and validated empirically. **Iter 3 state change:** the three mechanisms are now MERGED to `iteration/E1` (verified first-hand — see Findings), so the instrument is no longer waiting on the handoff; the formal execution pass runs against the fixtures and the verdicts land in the PoC results ledger.

**R001 validation bar (stakeholder decisions, Elab Iter 2 + Iter 2 verdict gate — governs this revision):** the bar is **behavioural, not statistical**, and it is now **FOUR clauses**. The prior ">90% of sampled users per office" figure is dropped — it is invented, and measured against a disposable directory the team seeds itself it cannot fail, so it proves nothing. The architectural risk is what the portal DOES when an attribute is absent. The four clauses, proven against deliberately-seeded gaps **and substitution-attempt fixtures**: **(a)** every employee is rendered whether or not their attributes are complete; **(b)** a missing attribute never removes someone from results; **(c)** a missing attribute never raises an error; **(d)** a missing attribute is displayed as missing — it is never replaced by a default, a placeholder, a guessed value, or another employee's value (stakeholder, verbatim, at the Iter 2 verdict gate). The first three clauses stop data from being LOST; the fourth stops it from being INVENTED. The stakeholder confirmed ("Yes") that the bar applies to **all four AD-reading use cases** — UC-004 (declared home, FR-010), UC-005 (FR-001), UC-006 (FR-002), UC-007 (FR-003) — so the R001 PoC exercises all four consumers through the one LDAP contract (INT-010, one graceful-degradation policy via INT-008). Real-AD data-quality measurement belongs to Construction (R011 residual, STK-004-dependent) and is excluded from the LCA evidence package.

**In scope (this iteration):** test case design for UC-001, UC-004, UC-010 (all flows: main + AF + EF), the R003 token-validation matrix, SEC-006/SEC-007 role enforcement, and the FOUR-clause R001 behavioural bar across all four AD-reading consumers (UC-004 AF-2; UC-005/006/007 AF-3) — with automation architecture, test data, and UC→TC traceability; **execution-state transition** of every case whose mechanism code exists in SCM (Iter 3 result: the three mechanisms are merged — the 19 mechanism-covered cases are Scripted with verdicts PENDING; see Findings).

**Out of scope (per Evaluation Mission):** full functional testing of all 10 UCs (Construction); execution against production AD/Keycloak (Construction, R010/R011); full-scale load testing (Construction); usability/adoption testing (AC-004, Transition pilot); UI visual-fidelity testing against CON-011 (Construction).

**[OMITTED: Test Plan — trigger not fired per Development Case §5.2 oracle (re-consulted this iteration, 2026-09-02: formal delivery / regulatory audit / contractual test reporting not in scope); per-iteration testing scope lives in the Iteration Plan and the Test Evaluation Summary.]**

### Measurable Testing Goals (per Quality Dimension)

Every goal is quantified from upstream artifacts — none invented here. Upstream thresholds marked `[ASSUMPTION — requires validation]` (2 s ignore window, queue capacity ≥ 10, sync ≤ 60 s, 95th-percentile basis) are treated by this test model as **the validation targets themselves**: the tests below are the empirical instrument that retires those assumptions. The R001 goal is the stakeholder-decided FOUR-clause behavioural bar — deliberately not a percentage.

| Quality Dimension | Measurable Goal | Threshold Source | Validated By |
|---|---|---|---|
| Reliability | 5-minute drop tolerated; queued events never lost; exact duplicates rejected; events ordered by recorded timestamp; sync ≤ 60 s after restore | REL-002, REL-003, AC-005, ADR-003 | TC-004, TC-005, TC-006, TC-020 |
| Functionality | One clocking event per press; status-aware button; six directory attributes displayed; unpublish = soft delete with record retained; **every AD-reading consumer renders every employee — blank fields on missing attributes, no removal, no error, and NO SUBSTITUTION (four-clause behavioural bar, all four consumers: the rendered/exported value is exactly blank — never a default, placeholder, guessed value, or another employee's value)** | FR-004, FR-009, FR-010, CON-012, FR-001, FR-002, FR-003 + R001 behavioural bar (stakeholder decisions, Elab Iter 2 + verdict gate) | TC-001…TC-003, TC-009…TC-011, TC-013…TC-016, TC-021…TC-023 |
| Performance | Clocking confirmation < 1 s on BOTH online and offline paths; LDAP query hard timeout 5 s | PRF-002, PRF-003, NFR-002 | TC-001, TC-004, TC-011, TC-012 |
| Security | OIDC token validated; roles extracted from claims; HR-only functions reject Employee-role sessions; employee sees only own clockings; no anonymous access | SEC-001, SEC-002, SEC-003, SEC-006, SEC-007, R003 | TC-007, TC-017, TC-018, TC-019 |
| Usability | Colleague's email + extension visible on the result card (no detail view); displayed times render America/Havana local, DST-aware — never raw UTC or server time | USA-003, USA-008, AC-003 | TC-008, TC-009, TC-011 |
| Data Integrity / Auditability | Timestamp captured at press, stored UTC, persisted unchanged on sync; audit entries append-only, atomic with the state change; category change audited with old + new values | DAT-001, DAT-002, NFR-005, AUD-003, AUD-004 | TC-004, TC-005, TC-008, TC-013, TC-016, TC-023 |

### Test Types Mapped to Quality Dimensions and Levels

| Test Type | Dimension | Level | Cases |
|---|---|---|---|
| Functional (mechanism validation) | Functionality | System + Integration | TC-001…TC-003, TC-009…TC-011, TC-013…TC-016, TC-021…TC-023 |
| Reliability (fault tolerance) | Reliability | System | TC-004…TC-006, TC-020 |
| Security (auth + authorization) | Security | Integration + System | TC-007, TC-017…TC-019 |
| Performance (latency + timeout) | Performance | System + Integration | assertions inside TC-001, TC-004, TC-011, TC-012 |
| Data integrity / audit | Auditability | Unit + Integration | TC-008, TC-016, TC-023 |
| Regression | All | All | every case — re-run after EVERY merged PR (mandatory policy, Test Evaluation Summary) |

Multi-level coverage is deliberate: unit level (FakeClock DST boundary, audit atomicity), integration level (stub issuer, disposable directory, real PostgreSQL — including the four-consumer R001 bar cases TC-011/TC-021…TC-023), system level (browser flows with network control). No level is skipped; no level is exclusive.

### Test Automation Architecture (stubs, drivers, fakes)

Test infrastructure is a deliverable, not a convenience. The component diagram below is the shared automation architecture referenced by every case's automation notes; test scripts and suites are code in `tests/EmployeePortal.Tests/` (co-owned with the Implementer), gated by CI on every push (CR-5 hard gate). **Iter 3: the harness is materialized** — `EmployeePortal.Tests.csproj` (sha 23b9d1) carries xunit 2.9.2 + Microsoft.NET.Test.Sdk 17.12.0 and references the portal project; the Implementer's dual-coverage suites (CR-2 verified by the Code Reviewer, CI green ×3) implement this architecture against the merged mechanisms.

```plantuml
@startuml
skinparam componentStyle rectangle
skinparam fontSize 11
title Test Automation Architecture - Elaboration Iteration 3\nStubs, drivers, fixtures and fakes - one LDAP contract, four AD-reading consumers (UC-004/005/006/007)\nFOUR-clause R001 bar - clause (d) verified against substitution-attempt fixtures (A-28)

package "Test Harness (tests/EmployeePortal.Tests)" {
  component "Unit Test Driver (xUnit)\nblack-box contract + white-box paths (CR-2)" as UNITD <<driver>>
  component "Integration Test Driver (xUnit)\nreal engine + fixtures" as INTGD <<driver>>
  component "System Test Driver (browser automation)\nUI flows + network control + localStorage" as SYSD <<driver>>
  component "DB Assertion Helper\nread-only verification via Npgsql" as DBAH <<driver>>
}

package "Validation Fixtures" {
  component "Stub OIDC Issuer (R003)\nsigned JWTs: Employee / HR Admin /\nexpired / bad signature / no roles" as STUBOIDC <<stub>>
  component "Disposable LDAP Directory (R001)\n64 synthetic entries, 3 offices,\ndeliberate gaps: extension / job title /\ndepartment + empty-string edge +\nunresolvable uid e099 (D-9) +\nSUBSTITUTION-ATTEMPT fixtures (A-28):\n'General' default dept, 'Central'\nfirst-office fallback, 'N/A' placeholder,\nfully-gapped entry" as LDAPFIX <<stub>>
  database "PostgreSQL dev instance\nreal engine (ADR-002, R008)" as PGDEV <<fixture>>
  component "Drop-Simulation Control\noffline / online switching (R004)" as DROPC <<driver>>
}

package "Unit-level Fakes (in-memory)" {
  component "FakeClock : ITimeConvention\nfixed instants incl. DST boundary" as FCLK <<fake>>
  component "FakePersistence : IPersistence\nin-memory staged changes" as FPERS <<fake>>
  component "FakeLdapGateway : ILdapGateway\ncanned entries incl. nulls" as FLDAP <<fake>>
}

package "System Under Test (src/EmployeePortal)" {
  component "OIDC Auth Middleware\n(COMP-006 / CLS-010)" as AUTH
  component "Clocking path\n(COMP-001/009/011 - CLS-001/008/011)" as CLKPATH
  component "Directory path\n(COMP-003/007 - CLS-003/009)" as DIRPATH
  component "HR AD-reading paths\n(COMP-004/010 via COMP-003 -\nCLS-004/006 resolving display data\nvia CLS-003, INT-008 GetDisplayData)" as HRPATH
  component "News + Audit path\n(COMP-002/005 - CLS-002/005)" as NEWSPATH
  component "Time Service\n(COMP-011 / CLS-007)" as TIMESVC
}

UNITD ..> FCLK
UNITD ..> FPERS
UNITD ..> FLDAP
UNITD ..> TIMESVC
UNITD ..> CLKPATH
INTGD ..> STUBOIDC
INTGD ..> LDAPFIX
INTGD ..> PGDEV
INTGD ..> AUTH
INTGD ..> DIRPATH
INTGD ..> HRPATH
INTGD ..> NEWSPATH
SYSD ..> STUBOIDC
SYSD ..> DROPC
SYSD ..> CLKPATH
SYSD ..> DIRPATH
SYSD ..> NEWSPATH
DBAH ..> PGDEV

AUTH ..> STUBOIDC : validates tokens (INT-011)
DIRPATH ..> LDAPFIX : LDAP v3 read-only (INT-010)
HRPATH ..> DIRPATH : display data via INT-008\n(one contract, four consumers)
CLKPATH ..> PGDEV : idempotent insert + sync (INT-016)
NEWSPATH ..> PGDEV : state + audit in one transaction (INT-015/019)

note bottom of STUBOIDC
  CON-004: NOT a real Keycloak realm.
  Proves the portal consumes and
  validates OIDC tokens and extracts
  roles from claims (R003).
end note

note bottom of LDAPFIX
  Stakeholder decision: a disposable
  directory, NOT production AD. Gaps
  are seeded deliberately (UC-004 S4)
  and the FOUR behavioural clauses
  are proven against them across
  UC-004/005/006/007 (TC-011,
  TC-021..TC-023). Clause (d) is
  proven against the substitution-
  attempt fixtures: the rendered or
  exported value must be BLANK -
  never "General", never "Central",
  never "N/A", never another
  employee's value. Production
  fidelity = R011 (Construction).
end note

note bottom of PGDEV
  Real engine required: ON CONFLICT
  idempotency (REL-002) and the
  append-only REVOKE (DAT-002) are
  engine semantics a fake cannot
  reproduce.
end note
@enduml
```

**Stub/driver justification (testability contract):** the stub OIDC issuer and disposable LDAP directory exist because the stakeholder explicitly refused to wait on STK-004 (R010) — R003 is proven by consuming tokens correctly, not by how the issuer got its users; R001 is a data-shape question answered by a disposable directory with deliberately-seeded gaps **and substitution-attempt fixtures**. The PostgreSQL dev instance is the REAL declared engine because idempotency (`ON CONFLICT`), the UNIQUE constraint, and the append-only REVOKE are engine semantics an in-memory fake cannot reproduce. The unit-level fakes exist because the Design Model made every subsystem boundary an interface (INT-006…INT-019) — the testability entry points are consumed here, not re-invented. The HR AD-reading cases (TC-021…TC-023) run at Integration level against the same disposable directory and the shared INT-008 GetDisplayData path, so the R001 PoC validates one contract across all four consumers without waiting for Construction UI.

### Test Workflow — UC Scenario to Executable Test Case

```plantuml
@startuml
title Test Workflow - UC Scenario to Executable Test Case (Elaboration Iteration 3)

start
:Load architecturally significant UCs\n(test priority 1): UC-001, UC-004, UC-010\n+ the R001 behavioural bar's four AD-reading\nconsumers (UC-004/005/006/007 AF-2/AF-3)\n+ the FOURTH clause (A-28): blank is the\nfinal value - never a default, placeholder,\nguess, or another employee's value;
:Walk every flow (main + AF + EF)\nwith adversarial intent:\nwhat failure does this test attack?;
:Design TC-NNN: preconditions, input data,\nexpected outcome, pass/fail criteria;
:Assign test level (unit / integration / system)\n+ automation feasibility per case;
:Map observable interface points (INT-006..INT-019)\n+ required stubs, drivers, fakes;
:Register UC-to-TC traceability\n(TC-001..TC-023, 23 cases);
if (Mechanism code handed off?\nF-CR-E1-1 RESOLVED Iter 3:\nmechanisms merged to iteration/E1) then (yes - current state)
  :Script the case in tests/EmployeePortal.Tests\n(regression-ready from creation;\nImplementer dual-coverage suites,\nCR-2 verified, CI green x3);
  :Execute against validation fixtures:\nstub OIDC issuer, disposable LDAP\ndirectory (substitution-attempt\nfixtures seeded), PostgreSQL dev,\ndrop simulation;
  if (All assertions hold?) then (yes)
    :Verdict PASSED;\nevidence: CI run + merged PR;
  else (no)
    :Verdict FAILED;\ndefect raised in SCM issue tracker;
  endif
  :Regression: re-run ALL prior results\nafter every merged PR (mandatory);
else (no - historical state, Cycles 1-2)
  :Verdict BLOCKED: designed + regression-ready;\nexecution waited on actions A-1..A-6\n(resolved Iter 3);
endif
:Results feed the Test Evaluation Summary\n(mission: R001 > R003 > R004, empirical);
stop
@enduml
```

**Test Evaluation Flow — Cycle 3 record (2026-09-02, this revision):**

```plantuml
@startuml
title Test Evaluation Flow - Elaboration Iteration 3, Cycle 1 (2026-09-02)

start
partition "S1 - DISCOVER" {
  :Load Review Record (Iter 3):\nA-28 assigned to Test Designer -\nextend TC-011 + TC-021/022/023 with\nfourth-clause verification steps +\nsubstitution-attempt fixtures,\ndeadline BEFORE TC execution;
  :Load the stakeholder's fourth clause,\nverbatim: "a missing attribute is\ndisplayed as missing. It is never\nreplaced by a default, a placeholder,\na guessed value, or another\nemployee's value.";
  :Load the Iter 3 code-review record:\n3 mechanism PRs APPROVED\n(base iteration/E1, CI green x3),\nF-CR-E1-1 RESOLVED;
}
partition "S2 - EXTEND THE INSTRUMENT (A-28)" {
  :TC-011 + TC-021/022/023 extended with\nASSERT-xd steps: the rendered or\nexported value for every missing\nattribute is EXACTLY blank;
  :Disposable LDAP fixture re-seeded with\nsubstitution-attempt fixtures:\n"General" default-department temptation,\n"Central" first-office fallback,\n"N/A" placeholder title,\nfully-gapped entry;
}
partition "S3 - VERIFY IMPLEMENTATION UNDER TEST" {
  :scm_get_file_content on iteration/E1:\nLdapGateway.cs sha b8df8b7 (four-clause\ndegradation incl. clause d),\nKeycloakAuthProvider.cs sha 7bd4cfd,\nClockingsRepository.cs sha 017cbcd,\noffline-queue.js sha 9ac644a,\nEmployeePortal.Tests.csproj sha 23b9d1\n(xunit 2.9.2 - the zero-package\nstate is gone);
  :The three mechanisms are MERGED to\niteration/E1 - the Cycle 1-2 blocking\ncause (Issue #1) is resolved at the\nsource;
}
partition "S4 - EXECUTION-STATE TRANSITION" {
  if (Mechanism code present in the build tree?) then (yes - first time)
    :19 mechanism-covered cases transition\nDesigned -> Scripted\n(Implementer dual-coverage suites,\nCR-2 verified, CI green x3);
    :Verdicts PENDING - none claimed:\nthe formal execution pass and the\nPoC results ledger own the verdicts;
    :TC-013..TC-016 remain BLOCKED on\nConstruction scheduling (news/audit\nis Construction scope - never an\nIssue #1 blocker);
  else (no)
    :BLOCKED verdicts (historical\nCycles 1-2 path);
  endif
}
partition "S5 - DEFECT CENSUS" {
  :Zero FAIL verdicts this revision ->\nzero new defects to formalize;
  :Issue #1 resolved at the source\n(F-CR-E1-1 RESOLVED, Review Record\nIter 3) - no duplicate raised;
}
:Record the transition + evidence in\nTest Case Findings (this artifact);\nMission verdict: NOT YET ACHIEVED -\nverdicts PENDING the formal execution\npass against the fixtures;
stop
@enduml
```

### Test Case Lifecycle

```plantuml
@startuml
title Test Case Lifecycle (TC-001..TC-023) - Elaboration Iteration 3

state "Designed" as DESIGNED
state "Scripted" as SCRIPTED
state "Blocked" as BLOCKED
state "Executed" as EXECUTED
state "Passed" as PASSED
state "Failed" as FAILED

[*] --> DESIGNED : case specified (this artifact):\npreconditions, input data, expected\noutcome, pass/fail criteria, UC trace

DESIGNED --> SCRIPTED : mechanism code handed off\n(F-CR-E1-1 RESOLVED Iter 3:\nmechanisms merged to iteration/E1)\n+ case scripted in tests/EmployeePortal.Tests\n(Implementer dual-coverage suites,\nCR-2 verified)
DESIGNED --> BLOCKED : code handoff absent\n(historical: Cycles 1-2, all 23 cases)

BLOCKED --> SCRIPTED : handoff arrives\n(RESOLVED Iter 3 - the 19 mechanism-covered\ncases transition; TC-013..TC-016 remain\nBLOCKED on Construction scheduling -\nnews/audit is Construction scope,\nnever an Issue #1 blocker)

SCRIPTED --> EXECUTED : execution pass runs against\nfixtures: stub OIDC issuer, disposable\nLDAP (substitution-attempt fixtures),\nPG dev, drop simulation\n(PENDING - no verdicts claimed yet)

EXECUTED --> PASSED : every assertion holds\n(evidence: CI run + merged PR)
EXECUTED --> FAILED : any assertion violated\n(defect raised in SCM tracker)

FAILED --> SCRIPTED : defect fix merged - re-run\n(thresholds are upstream-quantified;\nnever re-baselined to pass)
PASSED --> EXECUTED : regression re-run after\nEVERY merged PR (mandatory policy)

PASSED --> [*] : iteration closes; mission verdict\nrecorded (Test Evaluation Summary)
FAILED --> [*] : only by explicit Test\nManager deferral decision

DESIGNED : regression-ready from creation:\npreconditions + expected output\nfully specified
SCRIPTED : automated; runs in CI on\nevery push (CR-5 hard gate).\nCURRENT STATE (Iter 3): the 19\nmechanism-covered cases - verdicts\nPENDING, none claimed
BLOCKED : CURRENT STATE (Iter 3):\nTC-013..TC-016 only - news/audit\nmechanism is Construction scope\n(exit criteria 1-3 cover\nR001/R003/R004 only)
PASSED : evidence linked: CI run id\n+ merged PR (no fabricated results)
FAILED : defect lifecycle: NEW -> TRIAGED\n-> ... -> VERIFIED -> CLOSED

note right of SCRIPTED
  Iter 3 transition: the Cycle 1-2
  blocking cause (Issue #1 -
  mechanism code absent) is
  resolved at the source; the
  formal execution pass and the
  PoC results ledger own the
  verdicts - this artifact claims
  none.
end note
@enduml
```

**Execution status (Cycle 3 record, 2026-09-02):** the implementation under test was re-inspected empirically on `iteration/E1` — the three mechanisms are MERGED and present in the build tree: `LdapGateway.cs` (sha b8df8b7 — CLS-009 with the FOUR-clause graceful-degradation contract, clause (d) implemented as "missing or empty AD value → null; null is the FINAL mapped value"), `KeycloakAuthProvider.cs` (sha 7bd4cfd — CLS-010 with RS256/JWKS signature validation, exp/iss/aud/sub enforcement, verbatim role extraction), `ClockingsRepository.cs` (sha 017cbcd — interim in-memory adapter enforcing the UNIQUE idempotency_key contract, replaced by the PG adapter in Construction Iteration 1 per R008), `offline-queue.js` (sha 9ac644a — CLS-008 browser half: localStorage queue, capacity 10, press-time capture, sync on 200 OK), and `EmployeePortal.Tests.csproj` (sha 23b9d1 — xunit 2.9.2 + Microsoft.NET.Test.Sdk 17.12.0; the Cycle 1–2 zero-package state is gone). The 19 mechanism-covered cases (TC-001…TC-012, TC-017…TC-023) transition **Designed → Scripted** with verdicts **PENDING — none claimed**: the formal execution pass against the fixtures and the PoC results ledger own the verdicts, and this artifact fabricates no results. TC-013…TC-016 remain **BLOCKED on Construction scheduling** — the news/audit mechanism is Construction scope (exit criteria 1–3 cover R001/R003/R004 only), so they were never Issue #1 blockers. Full evidence in § Findings.
## Test Case Catalog
### Catalog Overview — UC→TC Traceability

| TC | UC / Flow | Level | Type | Automation | Priority |
|---|---|---|---|---|---|
| TC-001 | UC-001 main — clock in online | System | Functional + Performance | Browser automation + DB assert | 1 |
| TC-002 | UC-001 main — clock out, status-aware button | System | Functional | Browser automation + DB assert | 1 |
| TC-003 | UC-001 AF-3 — 2 s ignore window | System | Functional (adversarial) | Browser automation + DB assert | 1 |
| TC-004 | UC-001 AF-1 — queue during 5-min drop | System | Reliability | Browser automation + drop control | 1 (R004) |
| TC-005 | UC-001 AF-1 — sync on restore ≤ 60 s | System | Reliability | Browser automation + DB assert | 1 (R004) |
| TC-006 | UC-001 AF-1 — duplicate replay idempotency | Integration | Reliability (adversarial) | API driver + DB assert | 1 (R004) |
| TC-007 | UC-001 AF-2 — session expired → redirect | System | Security | Browser automation + stub issuer | 2 (R003) |
| TC-008 | UC-001 / DAT-001 / USA-008 — timestamp convention, DST boundary | Unit | Data integrity | xUnit + FakeClock | 1 |
| TC-009 | UC-004 main — search by name, six fields on card | Integration | Functional + Usability | API driver + disposable LDAP | 1 (R001) |
| TC-010 | UC-004 AF-1 — no results | Integration | Functional | API driver + disposable LDAP | 2 |
| TC-011 | UC-004 AF-2 / R001 — behavioural bar: deliberate gaps + substitution attempts, FOUR clauses | Integration | Functional (risk validation) | API driver + disposable LDAP | 1 (R001, HIGH) |
| TC-012 | UC-004 AF-3 — LDAP timeout, no local fallback | Integration | Performance + Reliability | API driver + fault injection | 1 |
| TC-013 | UC-010 main — unpublish = soft delete + audit | System | Functional + Audit | Browser automation + DB assert | 1 |
| TC-014 | UC-010 AF-1 — cancel: no change, no audit | System | Functional (adversarial) | Browser automation + DB assert | 2 |
| TC-015 | UC-010 AF-2 — already unpublished: option not offered | System | Functional (adversarial) | Browser automation | 2 |
| TC-016 | DAT-002 / AUD-003 — audit atomicity + append-only | Integration | Auditability (adversarial) | xUnit + PG dev + fault injection | 1 |
| TC-017 | SEC-006 — role denial on HR endpoints (UC-005/UC-010) | System | Security (adversarial) | API driver + stub issuer | 2 (R003) |
| TC-018 | SEC-007 — own-data-only history (UC-002 boundary) | System | Security (adversarial) | API driver + stub issuer | 2 |
| TC-019 | R003 / SEC-001/002/003 — token validation matrix | Integration | Security (risk validation) | API driver + stub issuer | 1 (R003) |
| TC-020 | UC-001 AF-1 / REL-002 — queue capacity ≥ 10 boundary | System | Reliability (adversarial) | Browser automation + drop control | 2 |
| TC-021 | UC-005 AF-3 / R001 — every event row rendered, blank display fields, no substitution | Integration | Functional (risk validation) | API driver + disposable LDAP + DB assert | 1 (R001, HIGH) |
| TC-022 | UC-006 AF-3 / R001 — every CSV row written, blank cells, no abort, no substitution | Integration | Functional (risk validation) | API driver + disposable LDAP + DB assert | 1 (R001, HIGH) |
| TC-023 | UC-007 AF-3 / R001 — employee locatable/selectable with blank fields, no substitution | Integration | Functional (risk validation) | API driver + disposable LDAP + DB assert | 1 (R001, HIGH) |

**Coverage check:** UC-001 — main (TC-001/002), AF-1 (TC-004/005/006/020), AF-2 (TC-007), AF-3 (TC-003), timestamp convention (TC-008). UC-004 — main (TC-009), AF-1 (TC-010), AF-2 (TC-011), AF-3 (TC-012). UC-005 — AF-3 (TC-021). UC-006 — AF-3 (TC-022). UC-007 — AF-3 (TC-023). UC-010 — main (TC-013), AF-1 (TC-014), AF-2 (TC-015), audit/soft-delete invariants (TC-016). Cross-cutting: SEC-006 (TC-017), SEC-007 (TC-018), R003/SEC-001/002/003 (TC-019). **The FOUR-clause R001 behavioural bar is covered on all four AD-reading consumers (TC-011 + TC-021/022/023), each with an adversarial clause walk against deliberately-seeded gaps AND substitution-attempt fixtures (A-28, Iter 3) — clause (d) can actually fail.** Every flow of the three priority UCs has at least one adversarial case; no case exists without a UC or declared-mechanism trace.

### Detailed Test Case Specifications

Each case names the **failure scenario it attacks** — the testing mindset is inversion: these cases exist to demonstrate the system does NOT work, and pass only when the attack fails.

---

**TC-001 — UC-001 main flow: clock in online (System, Functional + Performance)**

| Field | Value |
|---|---|
| Traces | UC-001 main flow; FR-004; PRF-002; DAT-001; SEQ-001 |
| Preconditions | E-003 (Employee, stub OIDC session) authenticated; no clocking event for E-003 today; portal server reachable; PostgreSQL dev instance clean for E-003 |
| Input data | Press "Clock In" at controlled instant T1 = 2026-09-01T12:58:12Z |
| Procedure | 1. Open Home (SCR-01). 2. Verify status chip "Not clocked in today" + green "Clock In" button. 3. Press button. 4. Measure press→confirmation latency. 5. Query `clockings` via DB Assertion Helper. |
| Expected outcome | Inline confirmation "Clocked in at 08:58:12" (America/Havana local, USA-008) visible **< 1 s** from press; exactly ONE row: employee_uid=e003, event_type=in, timestamp_utc=T1 (UTC), idempotency_key non-empty, synced_at_utc NULL; button flips to red "Clock Out"; status chip updates |
| Pass criteria | All assertions hold; latency < 1 s (PRF-002 online path) |
| Failure scenario attacked | Confirmation shown but row not persisted; timestamp captured at server-arrival time instead of press time (DAT-001 violation); confirmation > 1 s |
| Automation + interface points | Browser automation (system driver) + DB assert; INT-006 RecordEvent, INT-016 Add; fixtures: stub OIDC issuer, PG dev |

---

**TC-002 — UC-001 main flow: clock out, status-aware button (System, Functional)**

| Field | Value |
|---|---|
| Traces | UC-001 main flow steps 3–4; FR-004; SEQ-001 |
| Preconditions | E-003 has one persisted IN event (seeded, today); session valid |
| Input data | Reload Home; press "Clock Out" at T2 = T1 + 4 h |
| Procedure | 1. Reload Home. 2. Verify button reads "Clock Out" (red) — status-aware. 3. Press. 4. Query `clockings`. |
| Expected outcome | Button label/color matched the IN state BEFORE the press (never both buttons); second row: event_type=out, timestamp_utc=T2; confirmation < 1 s; history (UC-002 view) shows both events |
| Pass criteria | All assertions hold |
| Failure scenario attacked | Button not status-aware → wrong event type recorded (an OUT recorded while not clocked in, or vice versa) |
| Automation + interface points | Browser automation + DB assert; INT-006 GetCurrentStatus/RecordEvent; fixtures: stub OIDC, PG dev |

---

**TC-003 — UC-001 AF-3: repeated press within 2 s ignore window (System, adversarial)**

| Field | Value |
|---|---|
| Traces | UC-001 AF-3; FR-004; PRF-002 (window basis: 2 × response budget — upstream [ASSUMPTION] this test validates); SEQ-001 |
| Preconditions | E-003 not clocked in today; session valid |
| Input data | Two presses 0.8 s apart (inside the 2 s window); then, separately, a deliberate press 3 s after a first (outside the window) |
| Procedure | 1. Press "Clock In" at T1. 2. Press again at T1+0.8 s. 3. Query rows. 4. In a fresh state, press at T1, wait 3 s, press again — verify the second press IS processed as a new transition. |
| Expected outcome | Inside window: exactly ONE event (the stray press ignored, no opposite event). Outside window: the second press produces the next legitimate transition. |
| Pass criteria | 1 row after the double-press; no accidental opposite event; outside-window press behaves as a new transition |
| Failure scenario attacked | A stray second press producing an accidental opposite event (clock-in immediately followed by clock-out) — corrupting the attendance record |
| Automation + interface points | Browser automation + DB assert; INT-006 RecordEvent; fixtures: stub OIDC, PG dev |

---

**TC-004 — UC-001 AF-1: queue during 5-minute network drop (System, Reliability — R004)**

| Field | Value |
|---|---|
| Traces | UC-001 AF-1; NFR-004; AC-005; REL-002; PRF-002 (offline path); DAT-001; ADR-003; SEQ-001 |
| Preconditions | E-003 authenticated BEFORE the drop (session established); portal server then made unreachable via Drop-Simulation Control; `clockings` empty for E-003 |
| Input data | Press "Clock In" at T1; press "Clock Out" at T2 = T1 + 5 min (both during the outage) |
| Procedure | See procedure diagram below (Phases 1–3 shared with TC-005/TC-006) |
| Expected outcome | Confirmation from queued data **< 1 s** on BOTH presses (PRF-002 offline path); localStorage queue holds 2 events with RecordedAtUtc = press-time UTC (DAT-001) and idempotency keys, ordered by recorded timestamp (REL-002); ZERO rows in `clockings` during the outage |
| Pass criteria | All Phase-1 assertions hold (ASSERT-1…ASSERT-3) |
| Failure scenario attacked | Confirmation delayed > 1 s offline; timestamp captured at sync time instead of press time; event lost before sync |
| Automation + interface points | Browser automation + Drop-Simulation Control + DB assert; CLS-008 OfflineQueueClient (localStorage), INT-006; fixtures: stub OIDC, drop control, PG dev |

---

**TC-005 — UC-001 AF-1: sync on restore, ≤ 60 s, zero losses/duplicates (System, Reliability — R004)**

| Field | Value |
|---|---|
| Traces | UC-001 AF-1; AC-005; REL-002; REL-003; DAT-001; ADR-003; SEQ-001 |
| Preconditions | TC-004 Phase 1 complete: 2 events queued, outage held ≥ 5 minutes (AC-005 window) |
| Input data | Reconnect via Drop-Simulation Control |
| Procedure | See procedure diagram below (Phase 2) |
| Expected outcome | Sync completes ≤ 60 s from restore (REL-003): POST /api/clockings/sync → 200 OK; BOTH events persisted — zero losses, zero duplicates, ordered by recorded timestamp, timestamps UNCHANGED from press-time capture; localStorage queue cleared |
| Pass criteria | ASSERT-4…ASSERT-6 hold |
| Failure scenario attacked | Events lost or reordered on replay; sync exceeding 60 s; timestamps rewritten at sync time |
| Automation + interface points | Browser automation + DB assert; INT-006 SyncEvents, INT-016 AddRange, `uk_clockings_idempotency_key`; fixtures: drop control, PG dev |

---

**TC-006 — UC-001 AF-1: adversarial duplicate replay (Integration, Reliability — R004)**

| Field | Value |
|---|---|
| Traces | UC-001 AF-1; REL-002 conflict policy; ADR-003; SEQ-001 |
| Preconditions | TC-005 complete: both events persisted; the identical sync payload retained by the harness |
| Input data | POST the identical sync payload a second time (simulating a retry storm / double replay) |
| Procedure | See procedure diagram below (Phase 3) |
| Expected outcome | Row count UNCHANGED — `ON CONFLICT (idempotency_key) DO NOTHING` rejects exact duplicates; SyncResult: persisted=0, duplicatesRejected=2 |
| Pass criteria | ASSERT-7 holds; no new rows |
| Failure scenario attacked | Replay duplicating rows (idempotency broken) — the payroll record would count one clocking twice |
| Automation + interface points | API driver + DB assert; INT-006 SyncEvents; fixture: PG dev (real engine required — UNIQUE semantics) |

**Test Procedure — TC-004 / TC-005 / TC-006 (shared, AC-005 / R004):**

```plantuml
@startuml
title Test Procedure - TC-004/005/006: Offline Drop Validation (AC-005, R004)

start
partition "Phase 1 - simulate the 5-minute drop (TC-004)" {
  :Establish Employee session (stub OIDC, E-003)\nBEFORE the drop;
  :Drop-simulation control: portal server unreachable;
  :Press "Clock In" at controlled instant T1;
  :ASSERT-1 confirmation from queued data < 1 s\n(PRF-002 offline path);
  :ASSERT-2 localStorage queue holds 1 event:\nRecordedAtUtc = T1 (press-time UTC, DAT-001),\nidempotency key present, ordered (REL-002);
  :Press "Clock Out" at T2 (second queued event);
  :ASSERT-3 zero rows in clockings for E-003\nduring the outage;
}
partition "Phase 2 - restore and sync (TC-005)" {
  :Hold the outage >= 5 minutes (AC-005 window);
  :Reconnect;
  :ASSERT-4 sync completes <= 60 s (REL-003):\nPOST /api/clockings/sync -> 200 OK;
  :ASSERT-5 both events persisted: zero losses,\nzero duplicates, ordered by recorded timestamp,\ntimestamps unchanged (REL-002, DAT-001);
  :ASSERT-6 localStorage queue cleared;
}
partition "Phase 3 - adversarial replay (TC-006)" {
  :POST the identical sync payload again;
  :ASSERT-7 row count unchanged -\nON CONFLICT (idempotency_key) DO NOTHING;\nSyncResult: persisted = 0, duplicatesRejected = 2;
}
stop
@enduml
```

---

**TC-007 — UC-001 AF-2: session expired → redirect to Keycloak (System, Security)**

| Field | Value |
|---|---|
| Traces | UC-001 AF-2; SEC-001; SEQ-001 |
| Preconditions | E-003's stub-issued token EXPIRED (stub issuer emits an expired JWT); no valid session cookie |
| Input data | Navigate to Home with the expired session |
| Procedure | 1. Navigate to portal with expired token. 2. Observe redirect. 3. Complete re-authentication via stub issuer with a fresh Employee token. 4. Proceed to clocking. |
| Expected outcome | Redirect to the OIDC issuer login (EX-01) BEFORE any portal content renders; after re-auth, the clocking flow proceeds normally; at NO point is clocking data accessible with an expired token |
| Pass criteria | Redirect occurs; no content leak pre-auth; flow completes after re-auth |
| Failure scenario attacked | Expired session silently proceeding (auth bypass) — an unauthenticated actor recording clockings |
| Automation + interface points | Browser automation + stub issuer; INT-011 IAuthProvider; fixture: stub OIDC (expired-token variant) |

---

**TC-008 — DAT-001 / USA-008: timestamp convention across the DST boundary (Unit, Data Integrity)**

| Field | Value |
|---|---|
| Traces | UC-001 timestamp convention; DAT-001; USA-008; stakeholder decisions (store UTC, display America/Havana, export ISO-8601 with offset); CLS-007 TimeService; INT-014 |
| Preconditions | FakeClock injected as ITimeConvention (Design Model testability entry point); no database needed |
| Input data | Two fixed instants: summer S1 = 2026-07-15T12:00:00Z (Cuba DST in force) and winter W1 = 2026-12-15T12:00:00Z (standard time) |
| Procedure | 1. ToLocalDisplay(S1) → expect "08:00" (UTC-4). 2. ToLocalDisplay(W1) → expect "07:00" (UTC-5). 3. ToIso8601WithOffset(S1) → expect "2026-07-15T08:00:00-04:00". 4. ToIso8601WithOffset(W1) → expect "2026-12-15T07:00:00-05:00". 5. MonthBoundsLocal(2026, 9) → expect UTC bounds of the local September calendar days in America/Havana. |
| Expected outcome | Display strings differ by the DST offset between the two instants (08:00 vs 07:00 for the same 12:00Z); export strings carry the offset in force at each event time per the IANA zone database; month bounds are local calendar days, never the server's |
| Pass criteria | All five assertions hold; NO output renders raw UTC or a fixed -05:00 offset for the summer instant |
| Failure scenario attacked | A hardcoded UTC-5 offset — the exact defect the stakeholder warned about: it would silently shift every payroll day boundary when the clocks change. This case fails any implementation that ignores DST |
| Automation + interface points | xUnit unit driver + FakeClock; INT-014 ITimeConvention (all four operations); no fixtures beyond the fake |

---

**TC-009 — UC-004 main flow: search by name, six fields on the card (Integration, Functional + Usability)**

| Field | Value |
|---|---|
| Traces | UC-004 main flow; FR-010; USA-003; AC-003; SEQ-004 |
| Preconditions | Disposable LDAP directory loaded (64 entries, § Test Data); E-001 authenticated; target entry "Gómez, Elena" fully populated in office O1 |
| Input data | Search criteria: name = "Gómez" |
| Procedure | 1. Open Directory (SCR-04). 2. Enter name "Gómez". 3. Press Search. 4. Inspect result cards. |
| Expected outcome | All matching colleagues returned; the target card shows ALL SIX corporate fields on the card (name, job title, department, office, email, extension) — no detail view needed (USA-003); end-to-end ≤ 10 s (AC-003) |
| Pass criteria | Six fields present and correctly mapped per entry; no cross-mapped attribute (extension rendered as email, etc.); within time budget |
| Failure scenario attacked | Attribute cross-mapping in the LDAP result mapping (CLS-009 MapEntry) — a colleague's extension shown in the email field would send AC-003's "find a colleague's phone/email" wrong |
| Automation + interface points | API driver + disposable LDAP; INT-008 Search, INT-010 ILdapGateway.Search; fixture: disposable LDAP directory |

---

**TC-010 — UC-004 AF-1: no results (Integration, Functional)**

| Field | Value |
|---|---|
| Traces | UC-004 AF-1; FR-010; SEQ-004 |
| Preconditions | Disposable LDAP directory loaded; E-001 authenticated |
| Input data | Search criteria: name = "Zzzznonexistent" (matches nothing) |
| Procedure | 1. Enter the non-matching criteria. 2. Press Search. |
| Expected outcome | "No colleagues found" message with a suggestion to refine the search (P-05 empty state); NO error, NO blank page, NO partial card |
| Pass criteria | Message displayed; HTTP flow completes normally; no crash |
| Failure scenario attacked | Unhandled empty result → crash or blank page (the classic empty-set defect) |
| Automation + interface points | API driver + disposable LDAP; INT-008 Search; fixture: disposable LDAP directory |

---

**TC-011 — UC-004 AF-2 / R001: behavioural bar — deliberate gaps + substitution attempts, FOUR clauses (Integration, risk validation — HIGH)**

| Field | Value |
|---|---|
| Traces | UC-004 AF-2; FR-010; **R001 behavioural bar (stakeholder decisions, Elab Iter 2 + Iter 2 verdict gate — FOUR clauses; replaces the dropped >90% statistical criterion)**; USA-003; SEQ-004; CLS-009 MapEntry; INT-010 postcondition extension |
| Preconditions | Disposable LDAP directory loaded with DELIBERATE attribute gaps (§ Test Data: `ldap-o1-019` missing extension; `ldap-o2-007` missing job title; `ldap-o3-011` missing department — the UC-004 S4 bar walk; plus one empty-string attribute and the D-9 unresolvable uid e099) **AND substitution-attempt fixtures (A-28, Iter 3): `ldap-o1-017` missing department (a "General" default temptation), `ldap-o2-021` missing office (a "Central" first-office fallback), `ldap-o3-014` missing title (an "N/A" placeholder), `ldap-o1-018` fully-gapped (all display attributes missing)**; E-001 authenticated |
| Input data | One search per gapped/substitution-attempt entry's match criteria (name fragment matching each), plus one search matching all of them at once |
| Procedure | See procedure diagram below |
| Expected outcome | **Clause (a):** every gapped and substitution-attempt entry is RENDERED whether or not its attributes are complete. **Clause (b):** no entry is removed from the results for a missing attribute. **Clause (c):** a missing attribute never raises an error — missing fields render BLANK; the empty-string attribute renders blank too (null-vs-empty mapping edge in CLS-009 MapEntry); no cross-mapped attributes. **Clause (d):** the rendered value for every missing field is EXACTLY blank — `ldap-o1-017`'s department is NOT "General", `ldap-o2-021`'s office is NOT "Central", `ldap-o3-014`'s title is NOT "N/A", and the fully-gapped `ldap-o1-018` inherits NO attribute from any other entry. Blank is the answer — never a default, a placeholder, a guessed value, or another employee's value |
| Pass criteria | ASSERT-1a/1b/1c/1d hold for all gapped and substitution-attempt entries; empty-string edge renders blank; zero errors raised; zero substitutions observed |
| Failure scenario attacked | R001's declared failure mode: an entry with a missing attribute hidden entirely (the directory shows gaps), or a missing attribute raising an error that breaks the search — AND the substitution failure mode the fourth clause exists for: a missing department silently rendered as "General", a missing office as "Central" (the first office in the list), a missing title as "N/A", or a fully-gapped entry inheriting a colleague's attribute. The prior statistical criterion could not fail against self-seeded data; the behavioural clauses CAN fail — that is what makes them evidence |
| Automation + interface points | API driver + disposable LDAP; INT-008 Search, INT-010 ILdapGateway.Search + MapEntry; fixture: disposable LDAP directory with substitution-attempt fixtures (NOT production AD — production fidelity is R011, Construction) |

**Test Procedure — TC-011 (R001 behavioural bar, disposable directory, UC-004 declared home):**

```plantuml
@startuml
title Test Procedure - TC-011: R001 Behavioural Bar (HIGH risk, disposable directory, UC-004)\nFOUR clauses - clause (d) verified against substitution-attempt fixtures (A-28, Iter 3)

start
:Fixture: disposable LDAP directory - 64 synthetic\nentries, 3 offices (O1/O2/O3), deliberate gaps\nper UC-004 S4: ldap-o1-019 missing extension,\nldap-o2-007 missing job title, ldap-o3-011 missing\ndepartment; empty-string attribute (ldap-o2-013);\nuid e099 with NO AD entry (D-9 extreme);\nSUBSTITUTION-ATTEMPT fixtures (A-28):\nldap-o1-017 missing department ("General"\ndefault temptation), ldap-o2-021 missing office\n("Central" first-office fallback), ldap-o3-014\nmissing title ("N/A" placeholder), ldap-o1-018\nfully-gapped (all display attributes missing);
:Query via CLS-009 LdapGateway\n(INT-010 ILdapGateway.Search, LDAP v3 read-only);
if (Query succeeds within 5 s (PRF-003)?) then (yes)
  :Map results to CLS-026 DirectoryEntry\n(six corporate fields, FR-010);
  :ASSERT-1a clause (a): every gapped and\nsubstitution-attempt entry RENDERED -\nrendered whether or not complete;
  :ASSERT-1b clause (b): no entry removed\nfrom the results for a missing attribute;
  :ASSERT-1c clause (c): no error raised;\nmissing fields render BLANK;\nempty-string attribute renders blank\n(null-vs-empty edge, CLS-009 MapEntry);\nno cross-mapped attribute;
  :ASSERT-1d clause (d): the rendered value for\nevery missing field is EXACTLY blank -\nldap-o1-017 department is NOT "General",\nldap-o2-021 office is NOT "Central",\nldap-o3-014 title is NOT "N/A", and the\nfully-gapped ldap-o1-018 inherits NO\nattribute from any other entry\n(blank is the answer - never a default,\nplaceholder, guessed value, or\nanother employee's value);
else (no - UC-004 AF-3 path)
  :ASSERT-2 hard timeout at 5 s;\n"Directory temporarily unavailable";\nno local fallback (CON-006);
endif
:Record clause-by-clause results (a-d) as R001\nempirical evidence (LCA evidence package;\nreal-AD data quality -> Construction R011);
stop
@enduml
```

---

**TC-012 — UC-004 AF-3: LDAP timeout / connection failure, no local fallback (Integration, Performance + Reliability)**

| Field | Value |
|---|---|
| Traces | UC-004 AF-3; PRF-003 (5 s hard timeout); CON-006 (no local copy); SEQ-004 |
| Preconditions | LDAP fixture configured to HANG (accept connection, never respond) — fault injection |
| Input data | Search criteria: name = "Gómez" |
| Procedure | 1. Issue the search against the hanging directory. 2. Measure time to user-visible outcome. 3. Verify no cached/partial data is displayed. |
| Expected outcome | Query aborts at the 5 s hard timeout (PRF-003); "Directory temporarily unavailable" displayed (P-05); NO partial list, NO cached data (CON-006 forbids a local copy — there is nothing to fall back to) |
| Pass criteria | Timeout enforced at 5 s (not hanging indefinitely); unavailable message shown; zero entries rendered |
| Failure scenario attacked | Indefinite hang burning the AC-003 10-second budget; or a stale local cache displayed as if live (a CON-006 violation masquerading as fault tolerance) |
| Automation + interface points | API driver + fault-injected LDAP fixture; INT-010 ILdapGateway (timeout path); fixture: disposable LDAP in hang mode |

---

**TC-013 — UC-010 main flow: unpublish = soft delete + audit (System, Functional + Audit)**

| Field | Value |
|---|---|
| Traces | UC-010 main flow; FR-009; CON-012; NFR-005; AUD-003; DAT-002; SEQ-010 |
| Preconditions | HR-001 (HR Administrator, stub OIDC) authenticated; news item N-001 (published, featured, Events) exists; N-001 visible on SCR-03 |
| Input data | Press "Unpublish" on N-001; confirm in modal M-01 |
| Procedure | 1. Open News Management (SCR-08). 2. Verify "Unpublish" offered on N-001 (published). 3. Press; verify M-01 states the record is retained. 4. Confirm. 5. Query `news_items` + `news_audit` via DB assert. 6. Load News (SCR-03) as E-001. |
| Expected outcome | N-001 status = 'unpublished' (row STILL EXISTS — soft delete, CON-012); exactly ONE new `news_audit` row: action='unpublish', actor_uid=hr001, timestamp_utc set, snapshot present; N-001 hidden from SCR-03 (GetPublishedNews never returns it); record retained for audit |
| Pass criteria | All assertions hold; row count in `news_items` unchanged (no delete) |
| Failure scenario attacked | Hard delete instead of soft delete (CON-012 violation — the audit trail destroyed); or state change committed WITHOUT its audit entry (NFR-005 violation) |
| Automation + interface points | Browser automation + DB assert; INT-007 Unpublish, INT-012 AppendNewsAction, INT-015/INT-019; fixtures: stub OIDC, PG dev |

---

**TC-014 — UC-010 AF-1: cancel — no change, no audit entry (System, adversarial)**

| Field | Value |
|---|---|
| Traces | UC-010 AF-1; FR-009; NFR-005 (audits changes only); SEQ-010 |
| Preconditions | HR-001 authenticated; N-001 published; `news_audit` row count recorded |
| Input data | Press "Unpublish" on N-001; press Cancel in M-01 |
| Procedure | 1. Press Unpublish. 2. Cancel in the modal. 3. Query both tables. 4. Verify SCR-03 still shows N-001. |
| Expected outcome | N-001 status UNCHANGED ('published'); `news_audit` row count UNCHANGED (a cancelled action is not a change — NFR-005 audits changes); modal closed; item still visible to employees |
| Pass criteria | Zero state delta, zero audit delta |
| Failure scenario attacked | Cancel silently applying the unpublish, or writing a phantom audit entry — either corrupts the trail's meaning |
| Automation + interface points | Browser automation + DB assert; INT-007; fixtures: stub OIDC, PG dev |

---

**TC-015 — UC-010 AF-2: already unpublished — option not offered (System, adversarial)**

| Field | Value |
|---|---|
| Traces | UC-010 AF-2; FR-009; CON-012; CLS-022 state machine (Unpublished terminal); SEQ-010 |
| Preconditions | HR-001 authenticated; N-003 exists with status 'unpublished' (seeded) |
| Input data | Open News Management; inspect N-003's row; additionally, craft a direct POST to /hr/news/{N-003}/unpublish |
| Procedure | 1. Verify the UI offers NO "Unpublish" on N-003. 2. Craft the direct POST anyway (adversarial — UI hiding is never the only barrier). |
| Expected outcome | UI: no Unpublish affordance on the unpublished item. API: the crafted unpublish of an already-unpublished item does NOT create a second audit entry and does not corrupt state (idempotent no-op or explicit rejection — the state machine makes Unpublished terminal) |
| Pass criteria | No affordance; no new audit row; no state corruption from the crafted request |
| Failure scenario attacked | State machine violation — unpublishing an unpublished item (double audit entries for one logical action), or UI-only guarding (direct API call bypasses) |
| Automation + interface points | Browser automation + API driver + DB assert; INT-007 Unpublish precondition (item Status=Published); fixtures: stub OIDC, PG dev |

---

**TC-016 — DAT-002 / AUD-003: audit atomicity + append-only (Integration, adversarial)**

| Field | Value |
|---|---|
| Traces | DAT-002; NFR-005; AUD-003; UC-010; Process View audit atomicity rule; INT-019 (Add-only interface) |
| Preconditions | HR-001 authenticated; N-001 published; PG dev instance with baseline migration V1 applied (REVOKEs in force) |
| Input data | (a) Normal unpublish. (b) Fault-injected run: the audit INSERT is made to FAIL (e.g., constraint violation injected via the harness). (c) Attempted UPDATE and DELETE on `news_audit` as the application role. |
| Procedure | 1. Unpublish N-001 normally — verify item + audit committed in ONE transaction. 2. Re-run with the audit write faulted — verify the STATE CHANGE ROLLED BACK (no unpublish without its trail entry). 3. As the application role, attempt UPDATE and DELETE on `news_audit`. |
| Expected outcome | (a) Both rows committed together. (b) ZERO state change when the audit write fails — atomicity holds. (c) UPDATE and DELETE REJECTED by the engine (REVOKE per DAT-002) — the trail is physically append-only |
| Pass criteria | All three sub-assertions hold |
| Failure scenario attacked | A state change surviving a failed audit write (the trail lies by omission); or a mutable trail (an actor editing history) — both destroy NFR-005's mandatory traceability |
| Automation + interface points | xUnit integration driver + fault injection + DB assert; INT-012, INT-015 SaveChanges (single commit boundary), INT-019 Add-only; fixture: PG dev (real engine required — REVOKE semantics) |

---

**TC-017 — SEC-006: role denial on HR endpoints (System, adversarial)**

| Field | Value |
|---|---|
| Traces | SEC-006; UC-005 EF-1; UC-009 EF-1; UC-010; CON-004; SEQ-005/SEQ-009/SEQ-010 |
| Preconditions | E-003 authenticated with an EMPLOYEE-ONLY token (stub issuer: Employee role, no HR Administrator claim); N-001 published; clocking data exists for other employees |
| Input data | (a) Deep-link GET to /hr/clockings, /hr/news, /hr/categories. (b) Crafted POST to /hr/news/{N-001}/unpublish. (c) Crafted POST to /hr/clockings/export. |
| Procedure | 1. Attempt each GET — expect SCR-09 Access Denied. 2. Attempt each crafted POST — expect rejection BEFORE the controller executes. 3. Verify no data in any response body. |
| Expected outcome | Every HR path rejects the Employee-role session at the request boundary (middleware, before controller execution); SCR-09 rendered on GETs; NO clocking data for other employees, NO news management data, NO CSV content revealed in any response |
| Pass criteria | All requests rejected; zero data leakage in response bodies; rejection happens server-side (not UI-hiding) |
| Failure scenario attacked | Role enforcement implemented only as hidden navigation items — the classic bypass: a direct URL or crafted POST reaching the controller with an Employee token |
| Automation + interface points | API driver + stub issuer; INT-011 IAuthProvider (role mapping), SEC-006 enforcement point; fixture: stub OIDC (Employee-only token variant) |

---

**TC-018 — SEC-007: own-data-only clocking history (System, adversarial)**

| Field | Value |
|---|---|
| Traces | SEC-007; UC-002; FR-005; INT-006 GetHistory precondition (uid from claims, never from request); SEQ-002 |
| Preconditions | E-003 authenticated (Employee); E-001 has seeded clocking events; E-003 has its own distinct seeded events |
| Input data | (a) Normal GET /history as E-003. (b) Crafted request attempting to query E-001's history (e.g., parameter tampering: employeeUid=e001 in the request) |
| Procedure | 1. Load own history — verify only E-003's events. 2. Craft the request naming E-001's uid. 3. Inspect the response. |
| Expected outcome | (a) Only E-003's own events returned. (b) The tampered request returns E-003's OWN data (or an explicit rejection) — NEVER E-001's events; the uid is taken from the authenticated claims, never from the request |
| Pass criteria | Zero exposure of another employee's clocking data under any crafted request |
| Failure scenario attacked | IDOR — the uid read from the request instead of claims, exposing a colleague's attendance record to any employee |
| Automation + interface points | API driver + stub issuer; INT-006 GetHistory (SEC-007 precondition); fixtures: stub OIDC, PG dev |

---

**TC-019 — R003 / SEC-001/002/003: OIDC token validation matrix (Integration, risk validation)**

| Field | Value |
|---|---|
| Traces | R003; SEC-001; SEC-002; SEC-003; CON-004; INT-011 IAuthProvider; CLS-010 |
| Preconditions | Stub OIDC issuer emits five token variants (§ Test Data); portal configured as OIDC client of the stub |
| Input data | One request per variant: (V1) valid Employee token; (V2) valid HR Administrator token; (V3) expired token; (V4) bad-signature token; (V5) no token at all |
| Procedure | For each variant: attempt to load Home and one HR function; record accept/reject and the extracted roles. |
| Expected outcome | V1 → authenticated, Employee role extracted from claims, HR function denied. V2 → authenticated, HR Administrator role extracted, HR function allowed. V3 → redirect to issuer (AF-2 path). V4 → REJECTED (signature validation actually runs — not just decoded). V5 → challenge/redirect; NO anonymous access to any page (SEC-003) |
| Pass criteria | All five outcomes hold; roles come from claims (SEC-002), not from any portal-side assumption |
| Failure scenario attacked | Tokens accepted without signature validation (V4 — the highest-severity auth defect); roles not extracted (HR locked out or Employee elevated); anonymous access to any page |
| Automation + interface points | API driver + stub issuer; INT-011 ConfigureOidc/GetAuthenticatedUser; fixture: stub OIDC issuer (all five variants). This case IS the R003 empirical validation the stakeholder mandated — the portal consumes and validates OIDC tokens correctly, regardless of how the issuer got its users |

---

**TC-020 — UC-001 AF-1 / REL-002: queue capacity ≥ 10 boundary (System, adversarial)**

| Field | Value |
|---|---|
| Traces | UC-001 AF-1; REL-002 (per-client queue capacity ≥ 10 — upstream [ASSUMPTION] this test validates); ADR-003; CLS-008 |
| Preconditions | E-003 authenticated; portal unreachable (drop control); queue empty |
| Input data | 10 clocking presses during the outage (alternating in/out at 30 s intervals — the maximum the queue must hold) |
| Procedure | 1. Execute 10 presses during the outage. 2. Verify all 10 queue locally (capacity boundary). 3. Reconnect. 4. Verify all 10 sync with zero losses, zero duplicates, in recorded-timestamp order. |
| Expected outcome | All 10 events queued (none dropped at the capacity boundary); after restore, 10 rows persisted, correctly ordered, timestamps unchanged; sync ≤ 60 s |
| Pass criteria | 10/10 queued, 10/10 persisted, zero duplicates |
| Failure scenario attacked | Queue capacity below the declared ≥ 10 (events silently dropped at the boundary) — an employee's attendance record loses entries with no error shown |
| Automation + interface points | Browser automation + drop control + DB assert; CLS-008 (capacity = 10), INT-006 SyncEvents; fixtures: stub OIDC, drop control, PG dev |

---

**TC-021 — UC-005 AF-3 / R001: behavioural bar on the HR clocking review — every event row rendered, no substitution (Integration, risk validation — HIGH)**

| Field | Value |
|---|---|
| Traces | UC-005 AF-3 (stakeholder-confirmed, Elab Iter 2 — answer "Yes"; fourth clause added at the Iter 2 verdict gate, propagated Iter 3); FR-001; R001 behavioural bar (FOUR clauses); CON-005, CON-006; SEQ-005; CLS-017 (renders every event row); CLS-003 GetDisplayData (INT-008 postcondition extension); D-9 (unresolvable uid → all-null EmployeeDisplayData) |
| Preconditions | HR-001 authenticated (stub OIDC, HR Administrator token); disposable LDAP directory loaded with deliberate gaps AND substitution-attempt fixtures (§ Test Data); PostgreSQL seeded (seed S-4): clocking events this month for `ldap-o1-019` (missing extension), `ldap-o2-007` (missing job title), `ldap-o3-011` (missing department), `ldap-o1-017` (missing department — "General" temptation), `ldap-o2-021` (missing office — "Central" fallback), `ldap-o3-014` (missing title — "N/A" placeholder), `ldap-o1-018` (fully-gapped), and `e099` (uid with NO AD entry — the D-9 extreme), plus fully-populated control employees |
| Input data | Open the all-employees clocking review (SCR-05) with no filter — all events for the current month |
| Procedure | 1. GET the review data as HR-001. 2. Count rendered event rows; compare to the seeded event count. 3. Inspect the rows for the gapped, substitution-attempt, and e099 employees. 4. Contrast run: LDAP fixture unreachable → reload the review. |
| Expected outcome | **Clause (a):** EVERY event row is rendered — including rows for the gapped employees, the substitution-attempt employees, and e099 (D-9: an unresolvable uid maps to all-null display data; the row STAYS). **Clause (b):** no employee is removed from the review for a missing attribute. **Clause (c):** missing display fields render blank; no error raised. **Clause (d):** the display values for every missing field are EXACTLY blank — the department column is NOT "General", the office column is NOT "Central", the title column is NOT "N/A", and no row inherits an attribute from another employee's row. Clocking columns (event type, timestamp) are always complete — portal data from PostgreSQL, never AD data. **AF-2 contrast:** with LDAP unreachable, events remain viewable from PostgreSQL; the AD user id is shown and display attributes are marked unavailable — AF-2 (directory down) and AF-3 (attribute missing) are distinct contracts |
| Pass criteria | All four clauses hold for every gapped and substitution-attempt row; rendered row count == seeded event count; no error in either run; AF-2 contrast behaves per its own contract |
| Failure scenario attacked | An employee with missing AD attributes vanishing from the HR review — the review silently under-reports attendance (the exact R001 failure mode in the HR consumer); an unresolvable uid crashing the review load; AF-3 mis-implemented as AF-2 (the whole review blocked because one attribute is missing) — AND the substitution failure mode: a missing department silently rendered as "General" or a missing office as "Central" (the first office in the list), which reads as plausible data in an attendance report and is therefore worse than a blank |
| Automation + interface points | API driver + disposable LDAP + DB assert; INT-006 (ICLK review query), INT-008 GetDisplayData (postcondition extension), INT-010 ILdapGateway; fixtures: stub OIDC (HR token), disposable LDAP (substitution-attempt fixtures), PG dev |

---

**TC-022 — UC-006 AF-3 / R001: behavioural bar on the CSV export — every row written, blank cells, no abort, no substitution (Integration, risk validation — HIGH)**

| Field | Value |
|---|---|
| Traces | UC-006 AF-3 (stakeholder-confirmed, Elab Iter 2 — answer "Yes"; fourth clause added at the Iter 2 verdict gate, propagated Iter 3); FR-002; R001 behavioural bar (FOUR clauses); INT-005 / STD-003 (CSV column contract); CON-006 (ad_user_id always present); SEQ-006; CLS-006 ReportExportService; INT-013 postcondition extension; D-9 |
| Preconditions | HR-001 authenticated; disposable LDAP with deliberate gaps AND substitution-attempt fixtures; PostgreSQL seeded (seed S-4): the same month's events for the gapped employees, the substitution-attempt employees, e099, and control employees |
| Input data | Select the seeded month; Export CSV |
| Procedure | 1. Request the export. 2. Parse the CSV. 3. Count data rows; compare to the seeded event count. 4. Inspect the rows for the gapped, substitution-attempt, and e099 employees. 5. Verify event_timestamp format (ISO-8601 with explicit offset). 6. Contrast run: LDAP fixture unreachable → request the export again. |
| Expected outcome | **Clause (a):** EVERY event row is written — CSV data-row count == seeded event count, including the gapped employees, the substitution-attempt employees, and e099. **Clause (b):** no row dropped for a missing attribute. **Clause (c):** missing display fields (employee_name, department, office) are BLANK CELLS; no abort, no error. **Clause (d):** the parsed cells for every missing field are EXACTLY empty — the department cell is NOT "General", the office cell is NOT "Central", the title cell is NOT "N/A" (on the CSV that reaches payroll, a fabricated department is worse than an empty cell: an empty cell gets questioned, a plausible wrong one does not). ad_user_id (column 1) always present; event_timestamp ISO-8601 with explicit offset (America/Havana, DST-aware); event_type IN/OUT. **AF-2 contrast:** with LDAP unreachable the export ABORTS with "Directory temporarily unavailable" and NO partial file is produced — AF-2 (no identity resolvable at all) and AF-3 (identity resolved, display fields blank) are distinct contracts |
| Pass criteria | Row count exact; blank cells present on gapped rows; no abort; ad_user_id complete on every row; parsed cells exactly empty (zero substitutions); contrast run aborts with no partial file |
| Failure scenario attacked | The export aborting or dropping rows when one employee's department is missing — payroll loses a whole employee's attendance from the report; AF-3 swallowing AF-2 — a partial file with unresolved identities delivered to payroll — AND the substitution failure mode the fourth clause exists for: a fabricated department ("General") or office ("Central") written into the payroll CSV, where a plausible wrong value is worse than an empty cell because nobody questions it |
| Automation + interface points | API driver + disposable LDAP + DB assert; INT-013 (IReportExport), INT-008 GetDisplayData (via CLS-003), INT-014 (ITIME — ISO-8601 offset), INT-005/STD-003 (CSV column contract); fixtures: stub OIDC, disposable LDAP (substitution-attempt fixtures), PG dev |

---

**TC-023 — UC-007 AF-3 / R001: behavioural bar on worker category assignment — employee locatable and selectable, no substitution (Integration, risk validation — HIGH)**

| Field | Value |
|---|---|
| Traces | UC-007 AF-3 (stakeholder-confirmed, Elab Iter 2 — answer "Yes"; fourth clause added at the Iter 2 verdict gate, propagated Iter 3); FR-003; R001 behavioural bar (FOUR clauses); CON-006, CON-013; AUD-004, NFR-005; SEQ-007; CLS-020 CategoryController; CLS-004 CategoryService; INT-009 (ICAT), INT-008 (GetDisplayData) |
| Preconditions | HR-001 authenticated; disposable LDAP with deliberate gaps AND substitution-attempt fixtures; `ldap-o3-011` (missing department) and `ldap-o1-017` (missing department — "General" temptation) have NO current category mapping; `worker-categories.json` test copy loaded (FIXED list, CON-013) |
| Input data | Locate `ldap-o3-011` and `ldap-o1-017` in the employee lookup; select "Operational" from the fixed category list; confirm |
| Procedure | 1. Query the employee lookup for `ldap-o3-011` and `ldap-o1-017`. 2. Verify both entries render with the department blank — still locatable and selectable. 3. Verify the category select starts EMPTY for both (no default category pre-selected). 4. Select the category; confirm. 5. Query `worker_categories` + `category_audit` via DB assert. |
| Expected outcome | **Clause (a):** both employees are RENDERED — locatable and selectable with the missing display field blank. **Clause (b):** neither is hidden from the lookup. **Clause (c):** no error raised. **Clause (d):** the displayed department is EXACTLY blank — NOT "General" — and the category select starts EMPTY: no category is pre-selected or defaulted for a gapped employee (the "default category" temptation); only HR's explicit selection persists. Post-assignment: mapping persisted (ad_user_id → category, two columns only — CON-006); audit entry appended: actor + timestamp + old value + new value (AUD-004) |
| Pass criteria | All four clauses hold for both employees; mapping persisted; audit entry present with correct actor/old/new values |
| Failure scenario attacked | An employee with a missing attribute being unlocatable in the lookup — HR cannot assign a category and the assignment function silently loses people (the R001 failure mode in the category consumer; an employee nobody can select is an employee nobody can categorize); the assignment persisting WITHOUT its audit entry (NFR-005 violation) — AND the substitution failure mode: a missing department rendered as "General" letting HR select confidently against a fabricated identity, or a default category silently pre-selected so a gapped employee acquires a category HR never chose |
| Automation + interface points | API driver + disposable LDAP + DB assert; INT-009 (ICAT Assign), INT-008 GetDisplayData, INT-012 (IAUD append), INT-018 (worker_categories repository), INT-019 (category_audit, Add-only); fixtures: stub OIDC, disposable LDAP (substitution-attempt fixtures), PG dev |

**Test Procedure — TC-011 / TC-021 / TC-022 / TC-023 (shared, R001 behavioural bar — one contract, four consumers):**

```plantuml
@startuml
title R001 Behavioural Bar Validation - One Contract, Four Consumers\nTC-011 (UC-004) / TC-021 (UC-005) / TC-022 (UC-006) / TC-023 (UC-007)\nFOUR clauses - clause (d) against substitution-attempt fixtures (A-28, Iter 3)

start
partition "Fixture - deliberately seeded gaps + substitution attempts (stakeholder decisions, Elab Iter 2 + verdict gate)" {
  :Load disposable LDAP directory (64 entries, 3 offices)\nwith DELIBERATE gaps: ldap-o1-019 missing extension,\nldap-o2-007 missing job title, ldap-o3-011 missing\ndepartment (UC-004 S4) + empty-string attribute\n(null-vs-empty edge) + uid e099 with NO AD entry (D-9)\n+ SUBSTITUTION-ATTEMPT fixtures (A-28): ldap-o1-017\nmissing department ("General" temptation), ldap-o2-021\nmissing office ("Central" first-office fallback),\nldap-o3-014 missing title ("N/A" placeholder),\nldap-o1-018 fully-gapped;
  :Seed PostgreSQL: clocking events for the gapped\nemployees, the substitution-attempt employees,\nand e099 (unresolvable uid);
}
partition "TC-011 - UC-004 directory search (declared home, FR-010)" {
  :E-001 searches criteria matching all gapped and\nsubstitution-attempt entries;
  :ASSERT-1a all entries RENDERED (clause a);
  :ASSERT-1b none removed from the results (clause b);
  :ASSERT-1c missing fields render BLANK, no error (clause c)\n+ empty-string attribute renders blank;
  :ASSERT-1d rendered values EXACTLY blank - NOT "General",\nNOT "Central", NOT "N/A", no attribute inherited from\nanother entry (clause d - blank is the answer);
}
partition "TC-021 - UC-005 HR clocking review (FR-001)" {
  :HR-001 opens SCR-05; load all-employees events;
  :ASSERT-2a EVERY event row rendered - including rows\nfor gapped, substitution-attempt, and e099 entries\n(clause a; D-9: unresolvable uid -> all-null display\ndata, row stays);
  :ASSERT-2b no employee removed from the review (clause b);\nclocking columns always complete - portal data;
  :ASSERT-2c missing display fields blank, no error (clause c);
  :ASSERT-2d display values EXACTLY blank - department NOT\n"General", office NOT "Central", title NOT "N/A",\nno cross-employee inheritance (clause d);
  :AF-2 contrast: LDAP fixture unreachable ->\nevents STILL viewable from PostgreSQL;\nad_user_id shown, display attributes marked unavailable;
}
partition "TC-022 - UC-006 CSV export (FR-002)" {
  :HR-001 selects the seeded month, Export CSV;
  :ASSERT-3a EVERY event row written - CSV row count ==\nseeded event count (clause a);
  :ASSERT-3b no row dropped (clause b); ad_user_id\n(column 1) always present;
  :ASSERT-3c missing display fields = blank cells,\nno abort, no error (clause c; STD-003);\nevent_timestamp ISO-8601 with explicit offset;
  :ASSERT-3d parsed cells EXACTLY empty - the department\ncell is NOT "General", the office cell is NOT "Central",\nthe title cell is NOT "N/A" (clause d - on the CSV\nthat reaches payroll a fabricated department is\nworse than an empty cell);
  :AF-2 contrast: LDAP unreachable -> export ABORTS,\n"Directory temporarily unavailable", NO partial file;
}
partition "TC-023 - UC-007 worker category assignment (FR-003)" {
  :HR-001 opens SCR-06; locate gapped employee ldap-o3-011\nand substitution-attempt employee ldap-o1-017;
  :ASSERT-4a employees RENDERED - locatable and\nselectable with blank display fields (clause a);
  :ASSERT-4b not hidden from the lookup (clause b);
  :ASSERT-4c no error raised (clause c);
  :ASSERT-4d displayed department EXACTLY blank - NOT\n"General" (clause d); the category select starts\nEMPTY - no category is pre-selected or defaulted\nfor a gapped employee (the "default category"\ntemptation); only HR's explicit selection persists;
  :Select category from the FIXED list (CON-013), confirm;
  :ASSERT-5 mapping persisted: ad_user_id -> category\n(two columns only, CON-006); audit appended:\nactor + timestamp + old + new (AUD-004);
}
:Record clause-by-clause results (a-d) as R001 empirical evidence\n(LCA evidence package; real-AD data quality -> Construction R011);
stop
@enduml
```

---

**Cases deferred to Construction (recorded, not designed here):** UC-002/003/005/006/007/008/009 main-flow functional suites (the AF-3 R001-bar flows of UC-005/006/007 are designed NOW — TC-021…TC-023 — because they are part of the R001 PoC's empirical validation; their main flows remain Construction); PRF-001 full-scale page-load percentile measurement; USA-001/006/007/009 visual-fidelity and accessibility passes; AC-002/AC-004 usability tests. These trace to the Evaluation Mission's out-of-scope boundary and the Iteration Plan's Construction assignments — designing them now would exceed the Development Case's Elaboration test intensity (Medium).

### Findings — Elaboration Iteration 3, Cycle 1 (Execution-State Transition Record)

**Execution context (all values from actual tool calls, 2026-09-02 — nothing fabricated):**

| Item | Value | Source |
|---|---|---|
| **A-28 status** | **EXECUTED this revision** — TC-011 + TC-021/022/023 extended with fourth-clause verification steps (ASSERT-xd: the rendered/exported value is EXACTLY blank, never a default, placeholder, guessed value, or another employee's value); substitution-attempt fixtures seeded in the disposable LDAP fixture (§ Test Data). Deadline BEFORE TC execution — met: the extension lands in this revision, ahead of the formal execution pass | Review Record A-28 (stakeholder-contribution propagation chain); stakeholder fourth clause, verbatim |
| Implementation under test | `iteration/E1` — the three mechanisms are **MERGED and present**: `LdapGateway.cs` sha b8df8b7 (CLS-009, FOUR-clause graceful degradation — clause (d) implemented as "missing or empty AD value → null; null is the FINAL mapped value"), `KeycloakAuthProvider.cs` sha 7bd4cfd (CLS-010, RS256/JWKS signature validation, exp/iss/aud/sub enforcement, verbatim role extraction), `ClockingsRepository.cs` sha 017cbcd (interim in-memory adapter enforcing the UNIQUE idempotency_key contract; PG adapter lands Construction Iteration 1 per R008), `offline-queue.js` sha 9ac644a (CLS-008 browser half: localStorage queue, capacity 10, press-time capture, sync clears on 200 OK) | `scm_get_file_content("iteration/E1")` ×4 |
| Test harness | `EmployeePortal.Tests.csproj` sha 23b9d1 — xunit 2.9.2 + Microsoft.NET.Test.Sdk 17.12.0 + project reference to the portal; **the Cycle 1–2 zero-package state is gone** — the harness is materialized | `scm_get_file_content("iteration/E1")` |
| Delivery chain (upstream record) | 3 ready-for-review branches handed off; 3 PRs opened base `iteration/E1` (#3 R001, #4 R003, #5 R004); CI green ×3 (runs 33615260971 / 33615945653 / 33616121855); 3 APPROVED terminal dispositions (reviews 5088169328 / 5088169517 / 5088169685); F-CR-E1-1 RESOLVED; merged to `iteration/E1` (verified first-hand by the file probes above) | Review Record, Iter 3 code-review-lens record |
| Verdict | **19 mechanism-covered cases: Designed → Scripted, verdicts PENDING — none claimed. TC-013…TC-016: BLOCKED on Construction scheduling.** The formal execution pass against the fixtures and the PoC results ledger own the verdicts; this artifact fabricates no results | This revision's probes + upstream record |

**Per-case state transition — Cycle 3 (2026-09-02):**

| Case group | Prior state (Cycle 2) | Current state (Cycle 3) | Basis |
|---|---|---|---|
| TC-001…TC-008, TC-020 (UC-001 clocking, offline queue, timestamp convention) | BLOCKED (Issue #1) | **Scripted — verdict PENDING** | R004 mechanism (CLS-008 offline-queue.js sha 9ac644a, ClockingsRepository sha 017cbcd) and R003 mechanism (CLS-010 sha 7bd4cfd) merged to `iteration/E1`; harness materialized (Tests.csproj sha 23b9d1) |
| TC-009…TC-012 (UC-004 directory) | BLOCKED (Issue #1) | **Scripted — verdict PENDING** | R001 mechanism (CLS-009 LdapGateway sha b8df8b7) merged; four-clause degradation contract present in code |
| TC-013…TC-016 (UC-010 news/audit) | BLOCKED (Construction scheduling) | **BLOCKED — unchanged** | News/audit mechanism is **Construction scope** (not an Elaboration WI-7…9 mechanism) — design complete; execution deferred with the mechanism. NOT an Elaboration exit-criterion blocker (exit criteria 1–3 cover R001/R003/R004 only) |
| TC-017, TC-018 (SEC-006/SEC-007 role enforcement) | BLOCKED (Issue #1) | **Scripted — verdict PENDING** | R003 mechanism merged — auth middleware exists to enforce roles (OidcMiddleware rejects at the request boundary) |
| TC-019 (R003 token validation matrix) | BLOCKED (Issue #1) | **Scripted — verdict PENDING** | R003 mechanism merged — the empirical R003 validation the stakeholder mandated can now run |
| TC-021, TC-022, TC-023 (UC-005/006/007 AF-3 — FOUR-clause R001 bar) | BLOCKED (Issue #1) | **Scripted — verdict PENDING** | R001 mechanism + shared display-data path merged; **A-28 executed this revision** — fourth-clause steps and substitution-attempt fixtures in place BEFORE the execution pass |

**Honest verdict discipline:** the transition to Scripted is an observed state change (mechanisms merged, harness materialized — both verified first-hand via file probes). It is NOT an execution result. No PASS, FAIL, or duration is claimed for any case; the formal execution pass against the fixtures (stub OIDC issuer, disposable LDAP with substitution-attempt fixtures, PG dev, drop simulation) and the Architectural Proof-of-Concept results ledger own the verdicts. The R6 evidence gate requires clause-by-clause FOUR-clause × four-consumer R001 evidence (TC-011 + TC-021/022/023) — the instrument is now ready to produce exactly that evidence.

**Regression status:** still zero prior PASS results exist — the first execution has not occurred; there is nothing to re-run. The regression baseline activates with the first executed PASS; from that point the mandatory policy applies (re-run ALL prior results after EVERY merged PR).

**Cycle 3 verdict for the Evaluation Mission:** NOT YET ACHIEVED — exit criteria 1–3 (empirical R001/R003/R004 validation) await the formal execution pass and the observed results. What changed this cycle: (1) **A-28 executed** — the instrument now validates FOUR clauses × four consumers with substitution-attempt fixtures, so clause (d) can actually fail; (2) **the blocking cause is resolved at the source** — mechanisms merged, harness materialized, the 19 mechanism-covered cases Scripted. The remaining chain to the evidence package: formal execution pass → results into the PoC artifact § Results and Findings → Issue #1 closure on merged-PR evidence (owned by the Integrator/Architect chain, tracked by SAD F2 / Iteration Plan F3 — other lenses' findings).

**Test-code materialization status (Iter 3 update):** the harness is materialized (`EmployeePortal.Tests.csproj` sha 23b9d1, xunit 2.9.2) and the Implementer's dual-coverage suites implement this artifact's automation architecture (§ Test Automation Architecture) in `tests/EmployeePortal.Tests/` — CR-2 dual coverage verified by the Code Reviewer across all three PRs (black-box contract + white-box paths; the R001 bar suite covers all four consumers with deliberately-seeded substitution-attempt fixtures). The run is repeatable in CI per the mandatory regression policy.

### Findings — Elaboration Iteration 2, Cycle 1 (Execution Record — historical, preserved)

**Execution context (all values from actual tool calls, 2026-09-02 — nothing fabricated):**

| Item | Value | Source |
|---|---|---|
| Smoke test (build stability gate — Tester execution pass) | **PASS** — CI green on `main`, run 33550619216 (started 2026-09-01 19:37:50Z, completed 19:38:39Z) | `scm_get_build_status("main")` |
| Implementation under test | `iteration/E1` — `Program.cs` sha 5a1f720b0f03be897f524e9d1e8425440d5aa540 (bare Razor Pages boot: `AddRazorPages`/`MapRazorPages` only — no auth middleware, no service registrations) and `EmployeePortal.csproj` sha 9a04a31ebe4a98f731982c8ce0a74ba952e7b10d (zero package references — no Npgsql, no LDAP, no OIDC/JWT) — **byte-identical to the Cycle 1 inspection (2026-09-01)** | `scm_get_file_content("iteration/E1")` |
| Test-code state | `SmokeTests.cs` sha dc835d2b30f80ceb96a5cb296cb29364e52423e4 — single `Assert.True(true)`; CR-2 dual coverage 0/3 mechanisms | `scm_get_file_content("iteration/E1")` |
| **Mechanism branch probes (Tester execution pass)** | **Zero CI runs on ALL THREE mechanism branches**: `feature/E1-R001`, `feature/E1-R003`, `feature/E1-R004` — no code has been pushed at the source; the handoff is absent, not merely unmerged | `scm_get_build_status` ×3 |
| CI on `iteration/E1` | No runs found — zero pushes had landed on the integration branch | `scm_get_build_status("iteration/E1")` |
| Defect census (Tester execution pass) | **2 open issues**: #1 (blocker/critical) and #2 (minor/high) — **both `cr:approved` + `assigned:implementer`** (CCM triage complete; delivery pending) | `scm_list_issues` (all states) |
| Verdict | **TC-001…TC-023 all BLOCKED (23/23; zero PASS, zero FAIL, zero SKIP)** — confirmed by independent Tester re-inspection against fresh branch-level evidence | Issue #1 |

**Per-case verdicts — Cycle 2 (23/23 BLOCKED; independently confirmed by the Tester execution pass):**

| Case group | Verdict | Blocking cause (empirically confirmed) | CR |
|---|---|---|---|
| TC-001…TC-008, TC-020 (UC-001 clocking, offline queue, timestamp convention) | **BLOCKED** | R004 mechanism (CLS-008 OfflineQueueClient) and R003 mechanism (CLS-010) absent from the build tree: `EmployeePortal.csproj` (sha 9a04a31) had zero package references; `Program.cs` (sha 5a1f720) was a bare Razor Pages boot with no auth middleware | Issue #1 |
| TC-009…TC-012 (UC-004 directory) | **BLOCKED** | R001 mechanism (CLS-009 LdapGateway) absent: no LDAP package, no LDAP configuration | Issue #1 |
| TC-013…TC-016 (UC-010 news/audit) | **BLOCKED** | News/audit mechanism is **Construction scope** (not an Elaboration WI-7…9 mechanism) — design complete; execution deferred with the mechanism. NOT an Elaboration exit-criterion blocker (exit criteria 1–3 cover R001/R003/R004 only) | Construction scheduling |
| TC-017, TC-018 (SEC-006/SEC-007 role enforcement) | **BLOCKED** | R003 mechanism (CLS-010) absent — no auth middleware existed to enforce roles | Issue #1 |
| TC-019 (R003 token validation matrix) | **BLOCKED** | R003 mechanism absent — the empirical R003 validation the stakeholder mandated could not run | Issue #1 |
| TC-021, TC-022, TC-023 (UC-005/006/007 AF-3 — R001 behavioural bar, designed that iteration) | **BLOCKED** | R001 mechanism (CLS-009 LdapGateway) and the shared display-data path (CLS-003 GetDisplayData, INT-008 extension) absent — the four-consumer bar validation could not run | Issue #1 |

**Cycle 2 verdict for the Evaluation Mission:** NOT YET ACHIEVED — exit criteria 1–3 (empirical R001/R003/R004 validation) had no code evidence. What changed that cycle was the INSTRUMENT, not the verdict: TC-011 rewritten to the stakeholder-decided behavioural bar (the >90% statistical criterion dropped — invented, and against self-seeded data it cannot fail, so it proves nothing), the fixture re-seeded with deliberate gaps per UC-004 S4, and the R001 validation extended to all four AD-reading consumers (TC-011 + TC-021/022/023) as the stakeholder confirmed. The Tester execution pass added branch-level confirmation: zero CI runs on `feature/E1-R001`/`R003`/`R004` proved the handoff was absent at the source. Zero FAIL verdicts → zero new defects to formalize; the blocker was already Issue #1 with canonical CCM labels — no duplicate raised.

### Findings — Elaboration Iteration 1, Cycle 1 (Execution Record — historical, preserved)

**Execution context (all values from actual tool calls, 2026-09-01 — nothing fabricated):**

| Item | Value | Source |
|---|---|---|
| Smoke test (build stability gate) | **PASS** — CI green on `main`, run 33492338439 (started 2026-09-01 09:27:49Z, completed 09:28:38Z) | `scm_get_build_status("main")` |
| Implementation under test | `iteration/E1` — branch **EXISTS** (Review Record action A-1 DONE; it was absent at the Code Reviewer's cycle) but held 51 entries: skeleton only — no `Services/`, no `Infrastructure/`, no `worker-categories.json`, no `CONTRIBUTING.md` | `scm_get_repo_tree("iteration/E1")` |
| CI on `iteration/E1` | No runs found — zero pushes had landed on the branch | `scm_get_build_status("iteration/E1")` |
| CI trigger configuration | **VERIFIED CORRECT** — push + PR triggers on `main`, `iteration/**`, `chore/**`, `feature/**`, `hotfix/**` (sha 84443920ba9d87e9c1c675cdff1ab9a54bc21da5): the blocker was code delivery, NOT CI infrastructure | `scm_get_file_content(".github/workflows/ci.yml")` |
| Defect baseline before this cycle | 0 issues (all states) — the SCM tracker held no record of the two Review Record findings | `scm_list_issues` (all states) |

**Per-case verdicts — Cycle 1 (20/20 BLOCKED; zero PASS, zero FAIL, zero SKIP):**

| Case group | Verdict | Blocking cause (empirically confirmed) | CR |
|---|---|---|---|
| TC-001…TC-008, TC-020 (UC-001 clocking, offline queue, timestamp convention) | **BLOCKED** | R004 mechanism (CLS-008 OfflineQueueClient) and R003 mechanism (CLS-010) absent from the build tree: `EmployeePortal.csproj` (sha 9a04a31) had zero package references — no Npgsql, no LDAP, no OIDC/JWT; `Program.cs` (sha 5a1f720) was a bare Razor Pages boot with no auth middleware | Issue #1 |
| TC-009…TC-012 (UC-004 directory) | **BLOCKED** | R001 mechanism (CLS-009 LdapGateway) absent: no LDAP package, no LDAP configuration in `appsettings.json` (sha 10f68b8) | Issue #1 |
| TC-013…TC-016 (UC-010 news/audit) | **BLOCKED** | News/audit mechanism is **Construction scope** (not an Elaboration WI-7…9 mechanism) — design complete that iteration per WI-10; execution deferred with the mechanism. NOT an Elaboration exit-criterion blocker (exit criteria 1–3 cover R001/R003/R004 only) | Construction scheduling |
| TC-017, TC-018 (SEC-006/SEC-007 role enforcement) | **BLOCKED** | R003 mechanism (CLS-010) absent — no auth middleware existed to enforce roles | Issue #1 |
| TC-019 (R003 token validation matrix) | **BLOCKED** | R003 mechanism absent — the empirical R003 validation the stakeholder mandated could not run | Issue #1 |

**Reproduction notes (exact evidence, build ID captured):** build under test = `iteration/E1` @ file shas `Program.cs` 5a1f720b0f03be897f524e9d1e8425440d5aa540, `EmployeePortal.csproj` 9a04a31ebe4a98f731982c8ce0a74ba952e7b10d, `appsettings.json` 10f68b8c8b4f796baf8ddeee7551b6a52b9437cc, `SmokeTests.cs` dc835d2b30f80ceb96a5cb296cb29364e52423e4 (single `Assert.True(true)` — no mechanism tests, CR-2 dual coverage 0/3); smoke baseline = main CI run 33492338439 (GREEN). Expected (Iteration Plan WIs 7–9): evolutionary mechanism code in `src/` for R001/R003/R004 with dual-coverage tests on `feature/E1-{risk-id}` branches labeled `ready-for-review`. Actual: zero mechanism code, zero `ready-for-review` branches, zero PRs.

**Defects formalized in the SCM tracker (canonical CCM labels — heuristic #8):**

| Issue | Title | Labels | Traces |
|---|---|---|---|
| **#1** | R001/R003/R004 mechanism code absent from SCM — empirical validation of all Elaboration test cases BLOCKED | `change-request, cr:logged, nature:defect, severity:blocker, priority:critical` | Review Record F-CR-E1-1, actions A-2…A-4; Iteration Plan WIs 7–9, exit criteria 1–3; R001 (HIGH), R003, R004; TC-001…TC-023 |
| **#2** | CONTRIBUTING.md absent — programming-guidelines baseline missing for CR-1 review of the first mechanism PR | `change-request, cr:logged, nature:defect, severity:minor, priority:medium` | Review Record F-CR-E1-2, action A-5; code-review checklist CR-1 |
## Test Data
All data is synthetic and self-contained — no production AD data, no real employee identities. Fixtures are reusable Construction assets (R011 mitigation, Test Evaluation Summary recommendation 4).

### Test Identities (stub OIDC issuer)

| ID | uid | Role(s) | Purpose |
|---|---|---|---|
| E-001 | e001 | Employee | directory search actor (TC-009, TC-010, TC-011, TC-013 step 6) |
| E-002 | e002 | Employee | seeded history owner (TC-018 target) |
| E-003 | e003 | Employee | primary clocking actor (TC-001…TC-008, TC-017, TC-018, TC-020) |
| E-004 | e004 | Employee | reserve |
| HR-001 | hr001 | HR Administrator | news management actor (TC-013…TC-016); HR review/export/category actor (TC-021, TC-022, TC-023) |
| e099 | e099 | (no AD entry) | **D-9 extreme**: uid with clocking events in PostgreSQL but NO AD entry — an unresolvable uid must map to all-null display data and the row must stay (TC-021, TC-022) |

**Token variants (TC-007, TC-017, TC-018, TC-019, TC-021…TC-023):** V1 valid Employee (roles: ["employee"]); V2 valid HR Administrator (roles: ["employee","hr-administrator"]); V3 expired (valid signature, exp in the past); V4 bad signature (valid structure, wrong signing key); V5 absent (no Authorization header). Role claim names are the stub's configuration — the test asserts the PORTAL maps whatever claims the issuer sends (SEC-002), not a hardcoded claim path.

### Disposable LDAP Directory (R001 fixture — TC-009…TC-012, TC-021…TC-023)

64 synthetic entries, 3 offices (O1/O2/O3), 20 each. Six corporate attributes per entry (FR-010): displayName, title, department, office, mail, telephoneNumber (extension). **Re-seeded Iter 2 per the stakeholder's behavioural-bar decision and UC-004 S4 — the gaps are the point: they are seeded deliberately so the clauses can be proven to hold against them. Re-seeded AGAIN Iter 3 (A-28) with substitution-attempt fixtures — the fourth clause (no invented values) must be able to FAIL, so the fixture seeds the exact temptations a lazy implementation would take: a default department, a first-office fallback, a placeholder title, and a fully-gapped entry.** The prior population-rate column is dropped with the statistical criterion (a self-seeded rate measures our own test data — it cannot fail, so it proves nothing).

| Office | Entries | Deliberate gap | Purpose |
|---|---|---|---|
| O1 | 20 | `ldap-o1-019` missing telephoneNumber (extension) | TC-011 clause walk; TC-021 review row; TC-022 CSV row |
| O2 | 20 | `ldap-o2-007` missing title (job title) | TC-011 clause walk; TC-021 review row; TC-022 CSV row |
| O3 | 20 | `ldap-o3-011` missing department | TC-011 clause walk; TC-021 review row; TC-022 CSV row; **TC-023 assignment target** |

**Substitution-attempt fixtures (A-28, Iter 3 — clause (d) verification data):**

| Entry | Missing attribute | Substitution temptation seeded against | Verified by |
|---|---|---|---|
| `ldap-o1-017` | department | a "General" default department (the most plausible default a mapping layer could invent) | TC-011 ASSERT-1d; TC-021 ASSERT-2d; TC-022 ASSERT-3d; TC-023 ASSERT-4d |
| `ldap-o2-021` | office | a "Central" first-office fallback (the first office in the list — the fallback a lazy implementation takes) | TC-011 ASSERT-1d; TC-021 ASSERT-2d; TC-022 ASSERT-3d |
| `ldap-o3-014` | title | an "N/A" placeholder (the placeholder a rendering layer could substitute) | TC-011 ASSERT-1d; TC-021 ASSERT-2d; TC-022 ASSERT-3d |
| `ldap-o1-018` | ALL display attributes (fully-gapped) | cross-entry inheritance — a fully-gapped entry must inherit NO attribute from any other entry | TC-011 ASSERT-1d; TC-021 ASSERT-2d; TC-022 ASSERT-3d |

**Why these four temptations:** the stakeholder's rationale, verbatim — "Blank is an answer. 'General', or the first office in the list, is a fabrication — and on the CSV that reaches payroll a fabricated department is worse than an empty cell. An empty cell gets questioned. A plausible wrong one does not." Each fixture is the exact value a plausible-but-wrong implementation would produce; the ASSERT-xd steps fail if any of them appears. The fully-gapped entry attacks the cross-entry inheritance failure mode (a mapping bug that fills a blank from the previous row's data — the "another employee's value" arm of clause (d)).

Additional fixture edges: `ldap-o2-013` carries an **empty-string** telephoneNumber (distinct from absent) — exercises the null-vs-empty mapping edge in CLS-009 MapEntry (TC-011). **uid e099 has NO AD entry at all** (D-9 extreme) — clocking events exist for e099 in PostgreSQL; the review and export must still render/write its rows with all-null display data (TC-021, TC-022). Named search target: "Gómez, Elena" (`ldap-o1-003`, fully populated, department=O1) — the TC-009 happy-path hit. **This is NOT production AD** — production-instance fidelity is risk R011, retired in Construction integration (R010).

### News Fixture (TC-013…TC-016)

| ID | Title | Category | Featured | Status | Purpose |
|---|---|---|---|---|---|
| N-001 | "Summer event announcement" | Events | yes | published | TC-013 unpublish target (featured → banner disappears from SCR-03) |
| N-002 | "IT maintenance window" | IT | no | published | browse control (remains visible after N-001 unpublish) |
| N-003 | "Old HR policy note" | HR | no | unpublished | TC-015 already-unpublished target (retained record) |
| N-004 | "Welcome aboard" | General | no | published | reserve |

### Clocking Event Seeds

| Seed | Owner | Content | Purpose |
|---|---|---|---|
| S-1 | E-003 | one IN event today (timestamp_utc = today 12:00Z) | TC-002 clock-out precondition |
| S-2 | E-002 | 4 events this month (2 in, 2 out) | TC-018 own-data boundary target |
| S-3 | E-001 | 2 events this month | TC-017 leakage canary (must NEVER appear in E-003 responses) |
| **S-4 (Elab Iter 2; extended Iter 3)** | `ldap-o1-019`, `ldap-o2-007`, `ldap-o3-011`, `ldap-o1-017`, `ldap-o2-021`, `ldap-o3-014`, `ldap-o1-018`, `e099` | one IN + one OUT event each, current month (16 events total) | TC-021 review rows / TC-022 CSV rows for the gapped employees, the substitution-attempt employees, and the D-9 unresolvable uid — the FOUR-clause behavioural-bar data under test |

### Time Fixture (TC-008 — FakeClock instants)

| Instant | UTC | America/Havana (expected) | Why |
|---|---|---|---|
| S1 (summer) | 2026-07-15T12:00:00Z | 08:00:00, offset -04:00 (DST in force) | proves DST-aware display/export |
| W1 (winter) | 2026-12-15T12:00:00Z | 07:00:00, offset -05:00 (standard) | same 12:00Z must render differently — a fixed -05:00 fails here |
| Month bounds | September 2026 | local calendar days in America/Havana → UTC bounds | payroll day = local calendar day, never the server's |

### Offline Queue Data (TC-004…TC-006, TC-020)

Controlled press instants T1/T2 (TC-004/005: 2 events) and 10 alternating presses at 30 s intervals (TC-020: capacity boundary). Each press carries a harness-generated idempotency key (UUID) — the same keys are replayed verbatim in TC-006's duplicate-replay phase.

### Worker Category Fixture

`worker-categories.json` (ADR-004) test copy: ["Administrative", "Technical", "Operational"] — a representative FIXED list; no CRUD path exists anywhere in the portal (CON-013), so the fixture only needs to load and be read. TC-023 assigns "Operational" to `ldap-o3-011` and `ldap-o1-017`. **Clause (d) note (A-28):** the category select starts EMPTY for every employee — no default category is pre-selected for a gapped employee; only HR's explicit selection persists (the "default category" substitution temptation is seeded against, not with).
## Traceability
| Element | Traces From | Link Type | Traces To |
|---|---|---|---|
| Test Case artifact (this) | Use-Case Model (UC-001, UC-004, UC-010 — test priority 1; UC-005/006/007 AF-3 — R001 behavioural bar, stakeholder-confirmed Elab Iter 2); Supplementary Specification (PRF-002/003, REL-002/003, SEC-001/002/003/006/007, AUD-003/004, DAT-001/002, USA-003/008, INT-005, STD-003); SAD (COMP-001…011, ADR-003/004); Design Model (CLS-001…027, INT-006…019, D-9, testability entry points); Test Evaluation Summary (Evaluation Mission R001 > R003 > R004); Review Record (Risk List F1 — TC-011 named; F-CR-E1-1, actions A-1…A-6) | Tests | Elaboration exit criteria 1–3; LCA evidence package; Construction regression suite baseline |
| TC-001, TC-002 | UC-001 main flow, FR-004, PRF-002, DAT-001, SEQ-001 | Tests | CLS-001, CLS-007, CLS-017; INT-006, INT-016; COMP-001/011 |
| TC-003 | UC-001 AF-3, FR-004, PRF-002 (window basis) | Tests | CLS-017 (2 s ignore), INT-006 |
| TC-004, TC-005, TC-006, TC-020 | UC-001 AF-1, NFR-004, AC-005, REL-002, REL-003, PRF-002, DAT-001, ADR-003, R004 | Tests | CLS-008, CLS-001.SyncEvents, CLS-011; INT-006, INT-016; `uk_clockings_idempotency_key`; COMP-009 |
| TC-007 | UC-001 AF-2, SEC-001, R003 | Tests | CLS-010, INT-011; COMP-006 |
| TC-008 | DAT-001, USA-008, stakeholder decisions (UTC storage / America/Havana / ISO-8601 offset / local payroll day) | Tests | CLS-007, INT-014; COMP-011 |
| TC-009, TC-010 | UC-004 main/AF-1, FR-010, USA-003, AC-003, SEQ-004 | Tests | CLS-003, CLS-009, CLS-019; INT-008, INT-010; COMP-003/007 |
| TC-011 | UC-004 AF-2, FR-010, **R001 behavioural bar (stakeholder decision, Elab Iter 2 — three clauses; replaces the dropped >90% statistical criterion per Risk List F1 remediation)**, USA-003, UC-004 S4 bar walk | Tests | CLS-009 MapEntry, INT-010 (postcondition extension); COMP-007; disposable LDAP fixture with deliberately-seeded gaps (R001 empirical evidence) |
| TC-012 | UC-004 AF-3, PRF-003, CON-006 | Tests | CLS-009 (timeout path), INT-010; COMP-007 |
| TC-013, TC-014, TC-015 | UC-010 main/AF-1/AF-2, FR-009, CON-012, NFR-005, AUD-003, SEQ-010 | Tests | CLS-002, CLS-018, CLS-022 state machine; INT-007; COMP-002 |
| TC-016 | DAT-002, NFR-005, AUD-003, UC-010, Process View audit atomicity | Tests | CLS-005, CLS-011, INT-012, INT-015, INT-019; append-only REVOKE (migration V1); COMP-005/008 |
| TC-017 | SEC-006, UC-005 EF-1, UC-009 EF-1, UC-010, CON-004, R003 | Tests | CLS-010, INT-011; COMP-006; SCR-09 |
| TC-018 | SEC-007, UC-002, FR-005, INT-006 GetHistory precondition | Tests | CLS-017, CLS-001; INT-006 |
| TC-019 | R003, SEC-001, SEC-002, SEC-003, CON-004 | Tests | CLS-010, INT-011; COMP-006; stub OIDC issuer (R003 empirical evidence) |
| TC-021 | UC-005 AF-3 (stakeholder-confirmed, Elab Iter 2 — answer "Yes"), FR-001, R001 behavioural bar (three clauses), CON-005, CON-006, SEQ-005, D-9 | Tests | CLS-017, CLS-003 GetDisplayData (INT-008 postcondition extension), INT-006, INT-010; COMP-003; disposable LDAP fixture (R001 empirical evidence, HR review consumer) |
| TC-022 | UC-006 AF-3 (stakeholder-confirmed, Elab Iter 2 — answer "Yes"), FR-002, R001 behavioural bar (three clauses), INT-005, STD-003, CON-006, SEQ-006, D-9 | Tests | CLS-006 ReportExportService, INT-013 (postcondition extension), INT-008, INT-014; COMP-010; disposable LDAP fixture (R001 empirical evidence, CSV consumer) |
| TC-023 | UC-007 AF-3 (stakeholder-confirmed, Elab Iter 2 — answer "Yes"), FR-003, R001 behavioural bar (three clauses), CON-006, CON-013, AUD-004, NFR-005, SEQ-007 | Tests | CLS-020, CLS-004, INT-009, INT-008, INT-012, INT-018, INT-019; COMP-004; disposable LDAP fixture (R001 empirical evidence, category consumer) |
| Automation architecture (§ Test Scope) | Design Model testability entry points (INT-006…INT-019 fakeable); Test Evaluation Summary test configurations; Review Record CR-2 (dual coverage), CR-5 (CI gate) | DependsOn | tests/EmployeePortal.Tests; CI (ci.yml); Implementer mechanism PRs (WIs 7–9) |
| Test data (§ Test Data) | FR-010 (six attributes), FR-006/FR-009 (news lifecycle), FR-001/FR-002/FR-003 (HR AD-reading consumers), ADR-004 (category list), stakeholder decisions (America/Havana; behavioural bar + deliberate gap seeding, Elab Iter 2), UC-004 S4, D-9 | Derives | Disposable LDAP directory (re-seeded with deliberate gaps), stub OIDC issuer, PG dev instance, FakeClock (reusable Construction fixtures — R011) |
| Findings — Cycle 2 execution record (§ Test Case Catalog) | Review Record Risk List F1 (TC-011 named — resolved this revision); `scm_get_file_content("iteration/E1")` 2026-09-02 (Program.cs 5a1f720, csproj 9a04a31, SmokeTests.cs dc835d2 — byte-identical to Cycle 1); **Tester execution pass: `scm_get_build_status` main run 33550619216 (smoke PASS) + zero CI runs on `feature/E1-R001`/`R003`/`R004` and on `iteration/E1`; `scm_list_issues` (Issues #1/#2 both cr:approved, assigned:implementer)**; Issue #1 open | DependsOn | Issue #1 (SCM tracker); Test Evaluation Summary quality metrics; Iteration Assessment; LCA evidence package |
| Findings — Cycle 1 execution record (§ Test Case Catalog, historical) | Review Record F-CR-E1-1; CI run 33492338439 (`scm_get_build_status`); `iteration/E1` tree + file shas 5a1f720/9a04a31/10f68b8/dc835d2/8444392 (`scm_get_repo_tree`, `scm_get_file_content`); defect baseline 0 (`scm_list_issues`) | DependsOn | Issue #1, Issue #2 (SCM tracker); Test Evaluation Summary quality metrics; Iteration Assessment |
| Issue #1 (CR — blocker/critical) | TC-001…TC-023 BLOCKED verdicts; Iteration Plan WIs 7–9, exit criteria 1–3; R001 (HIGH), R003, R004; Review Record actions A-2…A-4 | Derives | Implementer (mechanism delivery + dual-coverage test code); Code Reviewer (A-6); LCA evidence package |
| Issue #2 (CR — minor/medium) | Review Record F-CR-E1-2, action A-5; code-review checklist CR-1 | Derives | CONTRIBUTING.md commit; first mechanism PR review |
| Test Evaluation Flow diagram (Cycle 2, § Test Scope) | S1–S4 execution this cycle (discover; correct the instrument per Risk List F1; extend coverage to four consumers; honest BLOCKED verdicts) | Refines | Findings — Cycle 2 execution record; Test Evaluation Summary mission verdict |
| Tester execution-pass diagram (Cycle 2 Findings, this revision) | S2 smoke gate (`scm_get_build_status` main — PASS, run 33550619216); S3 evaluation (file shas + branch probes — zero CI runs on feature/E1-R001/R003/R004); S4 defect census (`scm_list_issues` — 2 open, both cr:approved + assigned:implementer; zero new CRs) | Refines | Findings — Cycle 2 execution record; Test Evaluation Summary mission verdict; Iteration Assessment |
| Execution BLOCKED status | Review Record F-CR-E1-1 (Critical), actions A-1…A-6; SCM Issue #1 (blocker), Issue #2 (minor); Test Evaluation Summary INC-1 | DependsOn | Integrator (A-1 — DONE); Implementer (A-2…A-4); Code Reviewer (A-6) — unblocks all 23 cases |
| Test Plan omission | Development Case §5.2 oracle (trigger not fired — re-consulted 2026-09-02) | DependsOn | Iteration Plan (per-iteration testing scope); Test Evaluation Summary (strategy, schedule, resources) |
