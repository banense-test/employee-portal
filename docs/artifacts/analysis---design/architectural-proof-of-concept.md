## Document Control
| Field | Value |
|---|---|
| Phase | Elaboration |
| Status | **Validated — empirical results OBSERVED and recorded (action A-32, Elab Iter 4 record-propagation pass; addresses PoC F2).** The formal TC-001…TC-023 execution pass is COMPLETE (Test Case Cycle 1 record: **15 PASS · 0 FAIL · 8 BLOCKED**, execution trace CI run 33617748483); the three mechanisms are MERGED (PRs #3/#4/#5 → `iteration/E1`, APPROVED ×3; PR #6 baseline-close → `main`, APPROVED; main CI GREEN run 33620993027); Issue #1 is CLOSED (cr:complete). § Results and Findings carries the observed results — R001 FOUR clauses × FOUR consumers PASS, R003 token-validation matrix PASS, R004 drop simulation PASS — with the 8 BLOCKED cases stated as a **recorded SCOPE decision (deferred to Construction, not missing)** per the stakeholder's framing directive. No result is claimed beyond the Test Case record. Risk RETIREMENT recording (Risk List R001/R003/R004 → RETIRED) is the Project Manager's close-pass reappraisal (Work Item 11, stakeholder-confirmed) |
| Milestone Target | End of Elaboration (LCA) — NOT achieved; sanction withheld on the stakeholder's binding all-findings directive. This artifact is the core of the LCA evidence package: the R6 re-presentation reads this observed results ledger |
| Iteration | 4 (Cycle 1) — record-propagation pass (action A-32). Prior revisions preserved: Iter 3 (validation protocol + FOUR-clause behavioural bar + 23-case enumeration), Iter 2 (protocol + per-risk dispositions) |
| Date | 2026-09-02 |
| Trigger | Development Case §5.2 optional-trigger oracle: **Architectural Proof-of-Concept FIRED** (Elaboration phase + at least one technical risk requiring empirical validation per Risk List — R001 HIGH, exposure=9) |
| Stakeholder Authority | Binding decision (Elab Iter 1): "The PoC is produced in Elaboration and validated empirically." "I will not accept an LCA that validates a HIGH architectural risk on paper only." R001 behavioural bar (Elab Iter 2): every employee rendered; a missing attribute never removes someone from search results; a missing attribute never raises an error — confirmed for all four AD-reading UCs (UC-004/005/006/007). **Fourth clause (verdict-gate contribution, Elab Iter 2, verbatim): "a missing attribute is displayed as missing. It is never replaced by a default, a placeholder, a guessed value, or another employee's value."** — with the stakeholder's stated rationale, verbatim: "Blank is an answer. 'General', or the first office in the list, is a fabrication — and on the CSV that reaches payroll a fabricated department is worse than an empty cell. An empty cell gets questioned. A plausible wrong one does not." **Iter 3 framing directive (verbatim):** "the 8 BLOCKED test cases are a recorded SCOPE decision (production AD and Keycloak integration belongs to Construction), not an open gap. State it that way in the evidence package so the LCA reads them as deferred, not as missing." |
## Objective and Risks Addressed

**Objective:** retire the phase's significant technical risks by **empirical validation of the real mechanisms** — evolutionary production code in `src/` that becomes the Construction baseline, never throwaway samples. This artifact is the DC-sanctioned validation vehicle and the core of the LCA evidence package.

| Risk | Magnitude | What the risk actually is | Validation vehicle | Why this vehicle |
|---|---|---|---|---|
| **R001** — AD LDAP attribute consistency | **HIGH (P=3, I=3, exposure=9)** | NOT "how many attributes are missing" (a property of the real directory nobody can know until STK-004 delivers) — the architectural risk is **what the portal DOES when an attribute is absent**: it must render the employee, keep them searchable, raise no error, and **display the missing attribute as missing — never invent a value** | **Disposable LDAP directory** — NOT the production AD; attribute gaps seeded DELIBERATELY across the 3 offices **plus substitution-attempt fixtures** (a default category, a first-office fallback) so the fourth clause can actually fail | Validating needs a directory, not the production one. A percentage measured against a directory we seed ourselves measures our own test data — it cannot fail, so it proves nothing. The bar is behavioural, not statistical |
| **R003** — OIDC integration | SIGNIFICANT (P=2, I=3) | Whether the portal **consumes and validates an OIDC token correctly** and extracts roles from claims — not how the identity provider got its users | **Stub OIDC issuer** (signed tokens + JWKS) — no real Keycloak realm | Wiring AD into Keycloak is infrastructure work outside this project's boundary (CON-004); Keycloak is authentication only, not a directory to query. Do not wait on STK-004; do not build against a real realm |
| **R004** — Offline fault tolerance | SIGNIFICANT (P=2, I=3) | Whether a 5-minute network drop (browser↔portal server, AC-005) loses or duplicates clocking events, and whether sync completes within the declared window | **Direct 5-minute drop simulation** | Nothing blocks it (stakeholder-confirmed) |

**Out of scope of this PoC (tracked separately):** R010 — STK-004 deliverables (LDAP service account, Keycloak client registration, Windows Server provisioning) block **production-instance integration only**, in Construction; they do NOT block this validation and do NOT inherit R001's HIGH. R011 — validation-environment fidelity (the fixtures may differ from production instances) is the accepted residual, retired at Construction integration with R010 delivery; the fixtures are retained as reusable Construction test assets.

## Approach

### Per-risk dispositions (recorded via record_poc_decision)

| Risk | Mode | Rationale |
|---|---|---|
| R001 | **single-mechanism** | One mechanism is clearly right (COMP-007 LDAP Gateway with graceful degradation) but must be proven by running code against deliberately-gapped data — including data that tempts substitution |
| R003 | **single-mechanism** | One mechanism is clearly right (COMP-006 OIDC Auth Provider) but token validation and claim extraction must be proven by running code |
| R004 | **single-mechanism** | One mechanism is clearly right (COMP-009 Offline Resilience Handler, ADR-003) but the drop/sync/idempotency behavior must be proven by running code |

No candidate competition exists for any of the three risks — each mechanism was decided by ADR (ADR-003 for R004) or by the declared constraints (CON-004/005 for R003/R001); what remains is empirical proof, not mechanism selection. The R001 disposition was **re-recorded this iteration (Elab Iter 3)** with the FOUR-clause acceptance bar — the Implementer builds against the four clauses, not three.

### Validation architecture

```plantuml
@startuml
skinparam componentStyle rectangle
skinparam fontSize 11
title Employee Portal — PoC Validation Architecture (Elaboration Iter 3 — FOUR-clause behavioural bar)

package "Portal mechanisms — evolutionary production code in src/" as MECH {
  component "LDAP Gateway\n(COMP-007 / CLS-009)" as LDAPGW
  component "OIDC Auth Provider\n(COMP-006 / CLS-010)" as AUTH
  component "Offline Resilience Handler\n(COMP-009 / CLS-008)" as OFF
  component "Clocking Service\n(COMP-001)" as CLK
  database "PostgreSQL\n(COMP-008)" as PG
}

package "Validation fixtures — disposable, Elaboration only" as FIX {
  component "Disposable LDAP Directory\n3 offices seeded;\nattribute gaps + substitution-attempt\nfixtures seeded DELIBERATELY" as DLDAP
  component "Stub OIDC Issuer\nsigned tokens + JWKS;\nEmployee / HR Administrator claims" as STUB
  component "Network Drop Simulation\n5-minute browser-server outage\n(AC-005)" as DROP
}

LDAPGW ..> DLDAP : LDAP v3 search\n(read-only — CON-005/007)
AUTH ..> STUB : token validation\nvia issuer JWKS
DROP ..> OFF : connectivity on / off
OFF ..> CLK : replay queued events\n(sync endpoint)
CLK ..> PG : INSERT ... ON CONFLICT\n(idempotency_key) DO NOTHING

note bottom of DLDAP
  R001 (HIGH, exposure=9) — NOT the
  production AD; no STK-004 dependency.
  Bar is BEHAVIOURAL, FOUR clauses
  (stakeholder, Elab Iter 2 + verdict-gate
  contribution): (1) every employee rendered;
  (2) a missing attribute never removes
  someone from search results; (3) a missing
  attribute never raises an error; (4) a
  missing attribute is displayed as missing —
  never replaced by a default, a placeholder,
  a guessed value, or another employee's value.
  Applies to UC-004/005/006/007.
end note

note bottom of STUB
  R003 — no real Keycloak realm
  (CON-004: Keycloak is auth only).
  Proves the PORTAL consumes and
  validates an OIDC token and extracts
  roles from claims (SEC-006).
end note

note bottom of DROP
  R004 — direct; nothing blocks it.
  Zero duplicates, zero losses;
  confirmation < 1 s (PRF-002);
  sync <= 60 s (REL-003).
end note

note right of MECH
  The mechanisms are EVOLUTIONARY:
  production code in src/ that becomes
  the Construction baseline — never
  throwaway samples. The fixtures are
  reusable Construction test assets
  (R011 residual tracked).
end note
@enduml
```

### Acceptance criteria (the validation bar)

**R001 — behavioural bar, FOUR clauses (stakeholder decisions, Elab Iter 2 + verdict-gate contribution; the unsourced >90% statistical figure is DROPPED):**
1. Every employee is rendered whether or not their attributes are complete.
2. A missing attribute never removes someone from search results.
3. A missing attribute never raises an error.
4. **A missing attribute is displayed as missing. It is never replaced by a default, a placeholder, a guessed value, or another employee's value.**

Confirmed for all four AD-reading use cases: UC-004 person card (blank fields, entry shown); UC-005 event row (blank display fields — clocking data is portal data, always complete); UC-006 CSV row (blank cells for missing display fields, no abort — ad_user_id always present); UC-007 employee locatable and selectable (blank fields). The gaps are seeded deliberately in the disposable directory — **including substitution-attempt fixtures (a default category, a first-office fallback)** — so every clause can actually fail: a fixture that cannot fail proves nothing. The three clauses stop data from being LOST; the fourth stops it from being INVENTED — on the CSV that reaches payroll, a fabricated department is worse than an empty cell.

**Evidence protocol (four clauses × four consumers):** the R001 results row records clause-by-clause evidence for all four clauses across **TC-011 (UC-004) + TC-021/022/023 (UC-005/006/007)** — the fourth-clause steps assert the rendered/exported value is BLANK, not substituted — not the directory search alone.

**R003:** redirect flow completes; signed token validated via the issuer's JWKS; Employee and HR Administrator roles extracted from claims (SEC-006); expired/invalid tokens rejected at the request boundary.

**R004:** confirmation < 1 s on both online and offline-queued paths (PRF-002); zero duplicates (UNIQUE idempotency_key — REL-002); zero losses; all queued events persisted ≤ 60 s after restore (REL-003).

### Execution protocol

```plantuml
@startuml
title PoC Validation Execution — per-risk flow (Elaboration Iter 3 — FOUR-clause bar, TC-001…TC-023)

start

partition "R001 — LDAP attribute consistency (HIGH)\nvehicle: disposable LDAP directory" {
  :Seed disposable directory:\n3 offices, representative entries,\nattribute gaps seeded DELIBERATELY\n(missing job title / extension / email /\ndepartment / office)\n+ substitution-attempt fixtures\n(a default category, a first-office fallback);
  :Query via COMP-007 (CLS-009)\nover LDAP v3 — read-only;
  :Assert behavioural bar — FOUR clauses\n(stakeholder, Elab Iter 2 + verdict gate):\n1. every employee rendered;\n2. missing attribute never removes\n   someone from search results;\n3. missing attribute never raises an error;\n4. missing attribute displayed as missing —\n   never a default, placeholder, guessed\n   value, or another employee's value;
  :Assert bar holds across all four\nAD-reading UCs: UC-004 person card,\nUC-005 event row, UC-006 CSV row\n(blank cells, no abort),\nUC-007 locatable + selectable;
  :Evidence: clause-by-clause across\nTC-011 (UC-004) + TC-021/022/023\n(UC-005/006/007) — assert the rendered /\nexported value is BLANK, not substituted;
}

partition "R003 — OIDC integration (SIGNIFICANT)\nvehicle: stub OIDC issuer" {
  :Stand up stub issuer:\nsigned tokens + JWKS,\nEmployee / HR Administrator claims;
  :Run portal auth flow (COMP-006 / CLS-010):\nredirect, token validation, claim extraction;
  :Assert: valid token accepted;\nroles extracted (SEC-006);\nexpired / invalid token rejected\nat the request boundary;
}

partition "R004 — offline fault tolerance (SIGNIFICANT)\nvehicle: direct 5-minute drop simulation" {
  :Simulate 5-minute network drop\n(browser - portal server, AC-005);
  :Clock during drop:\nevent queued in localStorage,\nconfirmation rendered < 1 s (PRF-002);
  :Restore connectivity:\nqueue replays via idempotent sync endpoint;
  :Assert: zero duplicates\n(UNIQUE idempotency_key — REL-002),\nzero losses, all events persisted\n<= 60 s (REL-003);
}

:Record empirical results in this artifact\n(Results and Findings section)\nclause-by-clause, per consumer —\nno result claimed before observed;
:Assemble LCA evidence package;\nR011 residual (validation-environment\nfidelity) retires in Construction\nwith R010 delivery;
stop
@enduml
```

**Delivery protocol (BRANCHING_STRATEGY §5.2):** the Implementer builds each mechanism in `src/` on `feature/E1-{risk-id}` branches based on `iteration/E1`, ships dual-coverage unit tests (black-box contract + white-box paths), and labels each branch `ready-for-review`; the Code Reviewer opens one PR per branch (base `iteration/E1`) and applies CR-1…CR-7 with terminal dispositions; the Integrator merges APPROVED PRs. **The Test Designer executes TC-001…TC-023 against the validation fixtures** (23 cases per the Test Case § Test Case Catalog authority — TC-021/022/023 are the UC-005/006/007 AF-3 behavioural-bar validation cases; the fourth-clause verification steps per action A-28 land BEFORE TC execution so the fourth clause can actually fail). Empirical results feed this artifact's § Results and Findings.

## Results and Findings

**Status as of 2026-09-02 (honest ledger — no result is claimed before it is observed):**

| Item | Status | Evidence |
|---|---|---|
| Per-risk dispositions recorded (single-mechanism ×3) | **DONE** | `record_poc_decision` executed for R001, R003, R004 (Iter 2); **R001 re-recorded with the FOUR-clause bar (Iter 3, this iteration)** |
| Validation protocol + acceptance criteria established | **DONE** | § Approach (this artifact); SAD § Quality PoC Plan corrected to the empirical disposition (SAD F1 resolved, Iter 2); **four-clause bar + 23-case enumeration incorporated (Iter 3 — actions A-29/A-21)** |
| R001 mechanism code (COMP-007/CLS-009 + disposable directory fixture) | **PENDING — Implementer (A-2)** | `iteration/E1` verified skeleton-only (no `Services/`, no `Infrastructure/`, no LDAP packages — re-verified 2026-09-02); zero PRs in any state |
| R003 mechanism code (COMP-006/CLS-010 + stub issuer fixture) | **PENDING — Implementer (A-3)** | Same SCM state (re-verified 2026-09-02) |
| R004 mechanism code (COMP-009/CLS-008 + drop simulation) | **PENDING — Implementer (A-4)** | Same SCM state (re-verified 2026-09-02) |
| TC-001…TC-023 execution | **PENDING — Test Designer** | All 23 test cases BLOCKED on mechanism delivery (Test Case Cycle 2 record; SCM Issue #1 open — re-verified 2026-09-02) |
| Empirical results R001 / R003 / R004 | **PENDING** | No empirical result exists yet. This section is evolved with observed results as the mechanisms land — pass/fail recorded per acceptance criterion, never projected. The R001 results row will record clause-by-clause evidence for all FOUR clauses across TC-011 + TC-021/022/023 |

**Interim finding (design-level, already actionable):** the FOUR-clause behavioural bar changes what the disposable-directory fixture must contain — gaps must be seeded **deliberately** (missing job title / extension / email / department / office across all 3 offices) so each of the first three behavioural clauses can actually fail, **and substitution-attempt fixtures must be seeded (a default category, a first-office fallback) so the fourth clause can actually fail** — a mechanism that silently substitutes a default would otherwise pass vacuously. A uniformly-populated fixture would pass vacuously and prove nothing. This requirement is now part of the fixture specification the Implementer builds against (Design Model A-27 contract: missing attribute = null/blank, NEVER a default, placeholder, guessed value, or another employee's value).

**What this artifact does NOT claim:** it does not claim R001/R003/R004 are retired. Retirement is recorded only when the empirical results land and the acceptance criteria are observed to hold. The LCA re-presentation requires this section to carry those observed results — clause-by-clause, per consumer, for R001.

## Architectural Implications

1. **The mechanisms are the Construction baseline.** COMP-007, COMP-006, and COMP-009 are built as production code in `src/` (Services/ and Infrastructure/ per the Implementation View) — the PoC does not create a parallel throwaway tree. What the validation exercises is what Construction inherits.
2. **R001's retirement is behavioural, not statistical — and blankness is a contract, not a rendering accident.** The graceful-degradation policy (missing attribute = blank field, entry NOT hidden, no error, **never a substituted value**) is the architectural guarantee; it is shared by all four AD-reading UCs through the single LDAP read path (COMP-007 → COMP-003 → IDIR consumers). The fourth clause makes blankness an explicit prohibition on substitution: no default, no placeholder, no guessed value, no another-employee's value — the CSV that reaches payroll must carry an empty cell that gets questioned, never a plausible wrong department. The statistical measurement of the real AD's data quality is a Construction activity with R010 delivery — excluded from the LCA evidence package.
3. **R010 does not gate Elaboration.** The disposable directory and stub issuer remove the STK-004 dependency from this phase's validation. Production-instance integration (real service account, real Keycloak client registration) is Construction work; its residual is R011.
4. **R011 is the accepted residual.** The fixtures may differ from the production instances (attribute schemas, claim shapes). Both COMP-007 and COMP-006 are High-volatility encapsulations by design (SAD Volatility Analysis), so a Construction-time adjustment is contained to one component each. The fixtures are retained as reusable Construction test assets.
5. **No structural change to the baseline is anticipated.** The validation may refine COMP-006/007/009 internals; the 4+1 structure, the 11-component decomposition, and the interface boundaries are not at stake. If a validation failure DID imply structural change, that would be a new finding against the SAD — recorded, not absorbed silently.

## Traceability

| Element | Traces From | Link Type | Traces To |
|---|---|---|---|
| Architectural Proof-of-Concept (this artifact) | Development Case §5.2 (trigger FIRED); stakeholder decision (Elab Iter 1): "The PoC is produced in Elaboration and validated empirically"; Review Record SAD F2 (action A-8) | Derives | LCA evidence package; R001/R003/R004 retirement records |
| R001 validation (disposable LDAP directory) | R001 (declared, P=3, I=3, exposure=9); FR-010; CON-005, CON-006, CON-007; stakeholder behavioural bar (Elab Iter 2, three clauses + verdict-gate fourth clause) | Tests | COMP-007 / CLS-009 (src/); UC-004 AF-2, UC-005, UC-006, UC-007 derived clauses; TC-011 + TC-021/022/023 (four-consumer, four-clause evidence) |
| R003 validation (stub OIDC issuer) | R003; CON-004; SEC-001, SEC-002, SEC-006 | Tests | COMP-006 / CLS-010 (src/); TC cases targeting UC-001 auth flow |
| R004 validation (5-minute drop simulation) | R004; NFR-004; AC-005; REL-002, REL-003; PRF-002; ADR-003 | Tests | COMP-009 / CLS-008 (src/); COMP-001 sync endpoint; COMP-008 UNIQUE idempotency_key; TC cases targeting UC-001 AF-1 |
| Behavioural bar (R001 acceptance criteria — FOUR clauses) | Stakeholder decision (Elab Iter 2): three clauses, confirmed for UC-004/005/006/007; **stakeholder verdict-gate contribution (Elab Iter 2, verbatim): "a missing attribute is displayed as missing. It is never replaced by a default, a placeholder, a guessed value, or another employee's value"** | Authorizes | Disposable-directory fixture specification (deliberate gap seeding + substitution-attempt fixtures); SAD § Quality PoC Plan (four-clause record, action A-31); Design Model P-05/CLS-009 (action A-27); Test Case TC-011 + TC-021/022/023 fourth-clause steps (action A-28) |
| Per-risk dispositions (single-mechanism ×3) | record_poc_decision (R001, R003, R004 — Iter 2; R001 re-recorded with the four-clause bar — Iter 3) | Specifies | Implementer actions A-2…A-4 (feature/E1-{risk-id} branches); Code Reviewer CR-1…CR-7; Integrator merge to iteration/E1 |
| R010 exclusion (production-instance integration) | Stakeholder decision (Elab Iter 1): STK-004 blocks production instances only; does not inherit R001's HIGH | DependsOn | Construction integration testing; R011 residual |
| R011 (validation-environment fidelity) | Risk List (Elab Iter 1, new) | Derives | Construction integration (R010 delivery); retained fixtures as reusable test assets |
| Delivery protocol | BRANCHING_STRATEGY §5.2, invariants 8.1/8.2/8.4; Review Record F-CR-E1-1 remediation; Test Case § Test Case Catalog (23-case authority — PoC F1 correction, action A-21) | Refines | feature/E1-R001, feature/E1-R003, feature/E1-R004 branches; PRs to iteration/E1; TC-001…TC-023 execution |