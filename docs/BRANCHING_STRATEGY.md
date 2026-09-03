# Branching Strategy — Employee Portal

**Project:** Employee Portal (Cuba Corp)
**Phase:** Inception → Transition
**Current Phase:** Elaboration (Iteration 7 — record-correction pass)
**Maintainer:** Configuration Manager
**Last Updated:** 2026-09-03 (Elaboration Iter 7, Cycle 1 — E7 workspace CLOSED: iteration-close PR #15 merged to main (consolidated review APPROVED + main CI green, re-verified post-merge); tag `baseline-elaboration-E7-v1` written on merge SHA `a3b4fc9`; register row ESTABLISHED. E3-v1, E4-v1, E5-v1 and E6-v1 remain ESTABLISHED — E7-v1 is a distinct iteration baseline building on them, not a re-tag)

---

## 1. Purpose

This document defines the canonical branching model, naming conventions, baseline
pedigree gates, and cross-phase invariants for the Employee Portal project. It is
**config-as-code** — it lives in the repository and is consumed by every role
(Integrator, Implementer, Code Reviewer, Architect, Configuration Manager).

Updates to this file go **direct to `main`** via `scm_commit_files`. No PR is opened
for this file — it is documentation, not source code, and gating it behind a Reviewer
would block downstream consumers from reading updated conventions.

---

## 2. Configuration Items

| CI Category | Examples | Versioning |
|---|---|---|
| Source code | `src/` (.NET 10, Razor Pages) | Git commits on feature/iteration branches |
| Artifacts (RUP) | Vision, UC Model, SAD, Design Model, Test Plans | `upsert_artifact` → SCM-tracked Markdown |
| Configuration | `docs/BRANCHING_STRATEGY.md`, CI workflows, `appsettings.json` | Direct commit to `main` (docs) or PR (config affecting runtime) |
| Test data | Test fixtures, seed scripts | Git-tracked on feature branches |
| Baselines | Tags `baseline-{phase}{n}-v{x}` | Immutable Git tags |

---

## 3. Branch Naming Conventions

| Pattern | Phase | Purpose |
|---|---|---|
| `feature/I{n}-{subject}` | Inception | Feasibility mechanism built evolutionarily in `src/` (rare; only if risk reduction requires code) |
| `feature/E{n}-{risk-id}[-{mechanism}]` | Elaboration | Evolutionary architectural mechanism (e.g., `feature/E1-R001-ldap-attribute-mapping`) |
| `iteration/E{n}` | Elaboration | Integration workspace per Elaboration iteration |
| `feature/C{n}-{uc-id}-{subject}` | Construction | UC realization (e.g., `feature/C1-UC-004-clock-in-out`) |
| `iteration/C{n}` | Construction | Integration workspace per Construction iteration |
| `hotfix/{issue-id}` | Transition | Hotfix from `main` (e.g., `hotfix/CR-015`) |
| `chore/{subject}` | Any | Non-functional repo maintenance (branching strategy, CI config, dependency bumps) |

**Non-conforming branches** are surfaced as SCM issues with labels
`severity:minor`, `nature:defect`, `naming-violation`. The Configuration Manager
does NOT auto-rename — the branch owner corrects the name.

---

## 4. Branch Topology

The diagram below shows the workspace hierarchy: feature branches → integration
branches → `main`, with the review/merge flow for each phase.

```plantuml
@startuml
title IARI Branch Topology — Employee Portal

skinparam componentStyle rectangle
skinparam packageStyle rectangle

package "main (release branch)" {
    [main] as MAIN
}

package "Elaboration" {
    [iteration/E{n}] as ITER_E
    [feature/E{n}-{risk-id}] as FEAT_E1
    [feature/E{n}-{risk-id}-{mechanism}] as FEAT_E2
}

package "Construction" {
    [iteration/C{n}] as ITER_C
    [feature/C{n}-{uc-id}-{subject}] as FEAT_C1
    [feature/C{n}-{uc-id}-{subject}] as FEAT_C2
}

package "Transition" {
    [hotfix/{issue-id}] as HOTFIX
}

package "Inception" {
    [feature/I{n}-{subject}] as FEAT_I
}

' Elaboration flow
FEAT_E1 --> ITER_E : PR (Code Reviewer)
FEAT_E2 --> ITER_E : PR (Code Reviewer)
ITER_E --> MAIN : iteration-close PR (Architect review)

' Construction flow
FEAT_C1 --> ITER_C : PR (Code Reviewer)
FEAT_C2 --> ITER_C : PR (Code Reviewer)
ITER_C --> MAIN : iteration-close PR (Architect review)

' Transition flow
HOTFIX --> MAIN : express PR (Code Reviewer)

' Inception flow
FEAT_I --> MAIN : PR (Code Reviewer)

note right of MAIN
  Baseline tags:
  baseline-elaboration-E{n}-v{x}
  baseline-construction-C{n}-v{x}
  baseline-transition-T{n}-v{x}
end note

note left of ITER_E
  Integrator owns
  iteration/* branches
end note

note left of FEAT_E1
  Evolutionary mechanism
  (not throwaway PoC)
end note

@enduml
```

---

## 5. Per-Phase Branching Model

### 5.1 Inception (closed — LCO GO, Inception Iter 2)

- **Documentation only** — normally no implementation code.
- A feasibility mechanism, if genuinely required for risk reduction, is built
  **evolutionarily** in `src/` on `feature/I{n}-{subject}` (never throwaway).
- No baseline tags are written during Inception — architecture is not yet stable.
- RUP artifacts (Vision, UC Model, Supplementary Spec, SAD, Risk List, Iteration Plan)
  are the primary deliverables; they are persisted via `upsert_artifact` and tracked
  in SCM as Markdown.

### 5.2 Elaboration — Evolutionary Architectural Mechanism (Current Phase)

The architectural prototype is **evolutionary** — it becomes the Construction
baseline, not throwaway sample code.

- A technical risk is retired by **analysis** (the Software Architect reasons
  feasibility — no code) or by building the **real mechanism** in `src/` on
  `feature/E{n}-{risk-id}[-{mechanism}]` based on `iteration/E{n}`.
- The Architect records the decision as a process fact:
  `analysis-only` | `single-mechanism` | `candidates`.
- The Code Reviewer opens + reviews each mechanism PR (base `iteration/E{n}`) as
  production code.
- The Integrator merges the APPROVED mechanism into `iteration/E{n}`.
- For competing `candidates`, the Architect selects the winner and the Integrator
  closes the loser's PR per the recorded decision.
- At LAM (Late Elaboration close), the Integrator opens `iteration/E{n} → main`;
  the Architect reviews; the merge produces the Elaboration baseline.
- **There is no `samples/poc/` directory and no ephemeral `poc/*` branch.**

#### Baseline Identification Lifecycle — Elaboration E1 workspace (CLOSED, recorded 2026-09-02)

```plantuml
@startuml
title Baseline Identification Lifecycle — Elaboration E1 workspace CLOSED at Iteration 3 (position recorded 2026-09-02)

[*] --> WS
state "WorkspaceSetup\nIntegrator creates iteration/E1" as WS {
  WS : only the Integrator writes iteration/* (invariant 8.1)
  WS : iteration/E1 is the base of every mechanism PR
}
WS --> MW : workspace exists\n(DONE — created at Iter 1 close)
state "MechanismWork\nfeature/E1-{risk-id} -> iteration/E1" as MW {
  MW : Implementer built R001 / R003 / R004
  MW : mechanisms evolutionarily in src/ (no poc/ branch)
  MW : Code Reviewer opened + reviewed one PR
  MW : per mechanism (checklist CR-1..CR-7)
  MW : Integrator merged APPROVED PRs
}
MW --> LAM : all mechanisms integrated\n(DONE — Iter 3: PRs #3/#4/#5 APPROVED + merged)
state "LAMClosePR\niteration/E1 -> main" as LAM {
  LAM : opened by the Integrator at LAM close
  LAM : reviewed by the Architect
}
LAM --> GATE : PR #6 opened + APPROVED\n(DONE — 2026-09-02)
state GATE <<choice>>
GATE --> TAG : [review state == APPROVED\nAND main CI == green]\n(PASSED — both gates verified)
GATE --> BLK : [gate failure]
state "Tagged\nbaseline-elaboration-E3-v1" as TAG {
  TAG : scm_create_tag — audit message carries
  TAG : PR number + head SHA + review ID + CI URL
  TAG : register row PENDING -> ESTABLISHED
}
state "BlockerIssue\nseverity:blocker + nature:defect" as BLK {
  BLK : DO NOT tag — a tag on a red build
  BLK : or an unreviewed commit is a defect
  BLK : fix applied, then gates re-verified
}
BLK --> GATE : re-check both gates
TAG --> [*] : architecture baseline established\n(merge SHA c7915478)

note right of WS
  CLOSED POSITION (2026-09-02, Elab Iter 3 —
  convergence cycle, Cycle 1): the E1 workspace
  traversed every state. MechanismWork DONE:
  3 ready-for-review branches handed off
  (feature/E1-R001/R003/R004), 3 PRs opened
  base iteration/E1 (#3/#4/#5), 3 APPROVED
  terminal dispositions (reviews 5088169328 /
  5088169517 / 5088169685 — zero Critical,
  zero Major), merged by the Integrator.
  LAMClosePR DONE: PR #6 (iteration/E1 ->
  main) opened by the Integrator, consolidated
  review state APPROVED. GATE PASSED: review
  APPROVED + main CI green (run 33598979875).
  TAG WRITTEN: baseline-elaboration-E3-v1 on
  merge SHA c79154782f719c3e97b098cf3abd3ea83a3b553b.
end note

note right of TAG
  TAG NAMING (updated 2026-09-02): the
  iteration/E1 workspace closed at the END of
  Elaboration Iteration 3 (the convergence
  cycle absorbed the slipped Iter 1 and
  Iter 2 mechanism work). Per §7's
  iteration-number rule, the tag encodes the
  CLOSING iteration: baseline-elaboration-E3-v1.
  The workspace branch keeps its historical name
  iteration/E1 — branch names are minted at
  creation, never renamed. The never-written
  E1-v1 and E2-v1 anticipations are withdrawn:
  no tag was created under either name (Iter 1
  and Iter 2 closed NO-GO, dual gate
  unevaluable — no LAM-close PR existed).
  Re-tag (v2, v3...) only after an explicit
  rollback or post-baseline critical fix.
end note
@enduml
```

#### Baseline Identification Lifecycle — Elaboration E4 workspace (CLOSED, recorded 2026-09-02)

```plantuml
@startuml
title Baseline Identification Lifecycle — Elaboration E4 workspace CLOSED at Iteration 4 (position recorded 2026-09-02)

[*] --> WS4
state "WorkspaceSetup\nIntegrator creates iteration/E4" as WS4 {
  WS4 : only the Integrator writes iteration/* (invariant 8.1)
  WS4 : iteration/E4 is the base of the E4 correction PR
}
WS4 --> MW4 : workspace exists\n(DONE — created at Iter 4 open)
state "CorrectionWork\nfeature/E4-R003-state-comment -> iteration/E4" as MW4 {
  MW4 : Implementer built the F-CR-E3-3 remediation
  MW4 : evolutionarily in src/ (comment-only, 1 file +13/-2)
  MW4 : Code Reviewer opened + reviewed PR #7
  MW4 : checklist CR-1..CR-7 — zero findings
  MW4 : Integrator merged the APPROVED PR
}
MW4 --> LAM4 : correction integrated\n(DONE — PR #7 APPROVED review 5090059324 + merged)
state "LAMClosePR\niteration/E4 -> main" as LAM4 {
  LAM4 : opened by the Integrator at LAM close
  LAM4 : reviewed — consolidated state APPROVED
}
LAM4 --> GATE4 : PR #8 opened + APPROVED\n(DONE — 2026-09-02)
state GATE4 <<choice>>
GATE4 --> TAG4 : [review state == APPROVED\nAND main CI == green]\n(PASSED — both gates verified: review\nAPPROVED; main CI green run 33629662894,\nre-verified post-merge)
GATE4 --> BLK4 : [gate failure]
state "Tagged\nbaseline-elaboration-E4-v1" as TAG4 {
  TAG4 : scm_create_tag — audit message carries
  TAG4 : PR number + merge SHA + review chain + CI URL
  TAG4 : register row PENDING -> ESTABLISHED
}
state "BlockerIssue\nseverity:blocker + nature:defect" as BLK4 {
  BLK4 : DO NOT tag — a tag on a red build
  BLK4 : or an unreviewed commit is a defect
  BLK4 : fix applied, then gates re-verified
}
BLK4 --> GATE4 : re-check both gates
TAG4 --> [*] : architecture baseline established\n(merge SHA f47e99b8)

note right of TAG4
  TAG NAMING: the iteration/E4 workspace
  closed at the END of Elaboration Iteration 4
  (the record-propagation pass). Per §7's
  iteration-number rule, the tag encodes the
  CLOSING iteration: baseline-elaboration-E4-v1.
  The branch name and the closing iteration
  coincide — no naming reconciliation needed.
  baseline-elaboration-E3-v1 (merge SHA
  c7915478) REMAINS ESTABLISHED: E4-v1 is a
  distinct iteration baseline that BUILDS ON
  E3-v1 (E3 content + the E4 R003 state-comment
  correction), not a re-tag — no SUPERSEDED
  flip. Re-tag (v2, v3...) only after an
  explicit rollback or post-baseline critical fix.
end note

note right of WS4
  CLOSED POSITION (2026-09-02, Elab Iter 4,
  Cycle 1 — record-propagation pass): the E4
  workspace traversed every state.
  CorrectionWork DONE: PR #7 (feature/
  E4-R003-state-comment -> iteration/E4)
  APPROVED (review 5090059324, CI green run
  33632200967) and merged by the Integrator —
  F-CR-E3-3 RESOLVED (all three overstated-CSRF
  comment locations corrected with the honest
  [DEFERRED — lands with the session mechanism,
  Construction] marker). LAMClosePR DONE: PR #8
  (iteration/E4 -> main) opened by the
  Integrator, consolidated review state
  APPROVED (verified via
  scm_get_pull_request_review_state). GATE
  PASSED: review APPROVED + main CI green
  (run 33629662894, re-verified post-merge).
  TAG WRITTEN: baseline-elaboration-E4-v1 on
  merge SHA f47e99b814fd54a7317dcedbf682e1df8e9395c0.
end note
@enduml
```

#### Baseline Identification Lifecycle — Elaboration E5 workspace (CLOSED, recorded 2026-09-02)

```plantuml
@startuml
title Baseline Identification Lifecycle — Elaboration E5 workspace CLOSED at Iteration 5 (position recorded 2026-09-02)

[*] --> WS5
state "WorkspaceSetup\nIntegrator creates iteration/E5" as WS5 {
  WS5 : only the Integrator writes iteration/* (invariant 8.1)
  WS5 : iteration/E5 is the integration workspace
  WS5 : of the record-correction pass
}
WS5 --> MW5 : workspace exists\n(DONE — created at Iter 5 open)
state "IntegrationWork\nEMPTY integration set\n(record-correction pass)" as MW5 {
  MW5 : no code handoff entered the gate
  MW5 : 0 ready-for-review branches
  MW5 : 0 open PRs at cycle open
  MW5 : tree unchanged vs E4-v1
  MW5 : 3/3 mechanisms remain VERIFIED on main
}
MW5 --> LAM5 : integration set complete\n(DONE — empty set, nothing to merge)
state "LAMClosePR\niteration/E5 -> main" as LAM5 {
  LAM5 : opened by the Integrator at LAM close
  LAM5 : reviewed — consolidated state APPROVED
}
LAM5 --> GATE5 : PR #10 opened + APPROVED\n(DONE — 2026-09-02)
state GATE5 <<choice>>
GATE5 --> TAG5 : [review state == APPROVED\nAND main CI == green]\n(PASSED — both gates verified: review\nAPPROVED; main CI green run 33639518709,\nre-verified post-merge)
GATE5 --> BLK5 : [gate failure]
state "Tagged\nbaseline-elaboration-E5-v1" as TAG5 {
  TAG5 : scm_create_tag — audit message carries
  TAG5 : PR number + merge SHA + review chain + CI URL
  TAG5 : register row PENDING -> ESTABLISHED
}
state "BlockerIssue\nseverity:blocker + nature:defect" as BLK5 {
  BLK5 : DO NOT tag — a tag on a red build
  BLK5 : or an unreviewed commit is a defect
  BLK5 : fix applied, then gates re-verified
}
BLK5 --> GATE5 : re-check both gates
TAG5 --> [*] : architecture baseline established\n(merge SHA 58484d21)

note right of TAG5
  TAG NAMING: the iteration/E5 workspace
  closed at the END of Elaboration Iteration 5
  (the record-correction pass). Per §7's
  iteration-number rule, the tag encodes the
  CLOSING iteration: baseline-elaboration-E5-v1.
  The branch name and the closing iteration
  coincide — no naming reconciliation needed.
  baseline-elaboration-E4-v1 (merge SHA
  f47e99b8) REMAINS ESTABLISHED: E5-v1 is a
  distinct iteration baseline that BUILDS ON
  E4-v1 (E4 content + the E5 integration
  record; empty integration set), not a
  re-tag — no SUPERSEDED flip. E3-v1 (merge
  SHA c7915478) also remains ESTABLISHED.
  Re-tag (v2, v3...) only after an explicit
  rollback or post-baseline critical fix.
end note

note right of WS5
  CLOSED POSITION (2026-09-02, Elab Iter 5,
  Cycle 1 — record-correction pass): the E5
  workspace traversed every state.
  IntegrationWork DONE with an EMPTY set:
  the pass's work is artifact-record
  corrections owned by other roles (A-37
  Test Manager, A-38 Software Architect,
  A-39 Process Engineer) — no code handoff
  is owed, none entered the gate (0
  ready-for-review branches, 0 open PRs at
  cycle open; tree unchanged vs E4-v1).
  LAMClosePR DONE: PR #10 (iteration/E5 ->
  main) opened by the Integrator,
  consolidated review state APPROVED
  (verified via
  scm_get_pull_request_review_state).
  GATE PASSED: review APPROVED + main CI
  green (run 33639518709, re-verified
  post-merge). TAG WRITTEN:
  baseline-elaboration-E5-v1 on merge SHA
  58484d213fa199dbbd3c99472d6eed548b87e8c6.
end note
@enduml
```

#### Baseline Identification Lifecycle — Elaboration E6 workspace (CLOSED, recorded 2026-09-03)

```plantuml
@startuml
title Baseline Identification Lifecycle — Elaboration E6 workspace CLOSED at Iteration 6 (position recorded 2026-09-03)

[*] --> WS6
state "WorkspaceSetup\nIntegrator creates iteration/E6" as WS6 {
  WS6 : only the Integrator writes iteration/* (invariant 8.1)
  WS6 : iteration/E6 is the integration workspace
  WS6 : of the R014 record-correction cycle
}
WS6 --> MW6 : workspace exists\n(DONE — created at Iter 6 open)
state "IntegrationWork\nEMPTY integration set\n(R014 record-correction cycle)" as MW6 {
  MW6 : no code handoff entered the gate
  MW6 : 0 ready-for-review branches
  MW6 : 0 open PRs at cycle open
  MW6 : tree unchanged vs E5-v1
  MW6 : 3/3 mechanisms remain VERIFIED on main
}
MW6 --> LAM6 : integration set complete\n(DONE — empty set, nothing to merge)
state "LAMClosePR\niteration/E6 -> main" as LAM6 {
  LAM6 : opened by the Integrator at LAM close
  LAM6 : reviewed — consolidated state APPROVED
}
LAM6 --> GATE6 : PR #13 opened + APPROVED\n(DONE — 2026-09-02)
state GATE6 <<choice>>
GATE6 --> TAG6 : [review state == APPROVED\nAND main CI == green]\n(PASSED — both gates verified: review\nAPPROVED; main CI green run 33658332611,\nre-verified post-merge)
GATE6 --> BLK6 : [gate failure]
state "Tagged\nbaseline-elaboration-E6-v1" as TAG6 {
  TAG6 : scm_create_tag — audit message carries
  TAG6 : PR number + merge SHA + review chain + CI URL
  TAG6 : register row PENDING -> ESTABLISHED
}
state "BlockerIssue\nseverity:blocker + nature:defect" as BLK6 {
  BLK6 : DO NOT tag — a tag on a red build
  BLK6 : or an unreviewed commit is a defect
  BLK6 : fix applied, then gates re-verified
}
BLK6 --> GATE6 : re-check both gates
TAG6 --> [*] : architecture baseline established\n(merge SHA 264f5ec)

note right of TAG6
  TAG NAMING: the iteration/E6 workspace
  closed at the END of Elaboration Iteration 6
  (the R014 record-correction cycle). Per §7's
  iteration-number rule, the tag encodes the
  CLOSING iteration: baseline-elaboration-E6-v1.
  The branch name and the closing iteration
  coincide — no naming reconciliation needed.
  baseline-elaboration-E5-v1 (merge SHA
  58484d21) REMAINS ESTABLISHED: E6-v1 is a
  distinct iteration baseline that BUILDS ON
  E5-v1 (E5 content + the E6 integration
  record; empty integration set), not a
  re-tag — no SUPERSEDED flip. E4-v1 (merge
  SHA f47e99b8) and E3-v1 (merge
  SHA c7915478) also remain ESTABLISHED.
  Re-tag (v2, v3...) only after an explicit
  rollback or post-baseline critical fix.
end note

note right of WS6
  CLOSED POSITION (2026-09-03, Elab Iter 6,
  Cycle 1 — R014 record-correction cycle):
  the E6 workspace traversed every state.
  IntegrationWork DONE with an EMPTY set:
  the cycle's work is artifact-record
  corrections owned by other roles (A-40
  Test Manager, A-41 Process Engineer) —
  no code handoff is owed, none entered
  the gate (0 ready-for-review branches,
  0 open PRs at cycle open; tree unchanged
  vs E5-v1). LAMClosePR DONE: PR #13
  (iteration/E6 -> main) opened by the
  Integrator, consolidated review state
  APPROVED (verified via
  scm_get_pull_request_review_state).
  GATE PASSED: review APPROVED + main CI
  green (run 33658332611, re-verified
  post-merge). TAG WRITTEN:
  baseline-elaboration-E6-v1 on merge SHA
  264f5fec1fb03569156dc8607acb923ec1b08d01.
end note
@enduml
```

#### Baseline Identification Lifecycle — Elaboration E7 workspace (CLOSED, recorded 2026-09-03)

```plantuml
@startuml
title Baseline Identification Lifecycle — Elaboration E7 workspace CLOSED at Iteration 7 (position recorded 2026-09-03)

[*] --> WS7
state "WorkspaceSetup\nIntegrator creates iteration/E7" as WS7 {
  WS7 : only the Integrator writes iteration/* (invariant 8.1)
  WS7 : iteration/E7 is the integration workspace
  WS7 : of the record-correction pass
}
WS7 --> MW7 : workspace exists\n(DONE — created at Iter 7 open)
state "IntegrationWork\nEMPTY integration set\n(record-correction pass)" as MW7 {
  MW7 : no code handoff entered the gate
  MW7 : 0 ready-for-review branches
  MW7 : 0 open PRs at cycle open
  MW7 : tree unchanged vs E6-v1
  MW7 : 3/3 mechanisms remain VERIFIED on main
}
MW7 --> LAM7 : integration set complete\n(DONE — empty set, nothing to merge)
state "LAMClosePR\niteration/E7 -> main" as LAM7 {
  LAM7 : opened by the Integrator at LAM close
  LAM7 : reviewed — consolidated state APPROVED
}
LAM7 --> GATE7 : PR #15 opened + APPROVED\n(DONE — 2026-09-03)
state GATE7 <<choice>>
GATE7 --> TAG7 : [review state == APPROVED\nAND main CI == green]\n(PASSED — both gates verified: review\nAPPROVED; main CI green run 33711675908,\nre-verified post-merge)
GATE7 --> BLK7 : [gate failure]
state "Tagged\nbaseline-elaboration-E7-v1" as TAG7 {
  TAG7 : scm_create_tag — audit message carries
  TAG7 : PR number + merge SHA + review chain + CI URL
  TAG7 : register row PENDING -> ESTABLISHED
}
state "BlockerIssue\nseverity:blocker + nature:defect" as BLK7 {
  BLK7 : DO NOT tag — a tag on a red build
  BLK7 : or an unreviewed commit is a defect
  BLK7 : fix applied, then gates re-verified
}
BLK7 --> GATE7 : re-check both gates
TAG7 --> [*] : architecture baseline established\n(merge SHA a3b4fc9)

note right of TAG7
  TAG NAMING: the iteration/E7 workspace
  closed at the END of Elaboration Iteration 7
  (the record-correction pass). Per §7's
  iteration-number rule, the tag encodes the
  CLOSING iteration: baseline-elaboration-E7-v1.
  The branch name and the closing iteration
  coincide — no naming reconciliation needed.
  baseline-elaboration-E6-v1 (merge SHA
  264f5ec) REMAINS ESTABLISHED: E7-v1 is a
  distinct iteration baseline that BUILDS ON
  E6-v1 (E6 content + the E7 integration
  record; empty integration set), not a
  re-tag — no SUPERSEDED flip. E5-v1 (merge
  SHA 58484d21), E4-v1 (merge SHA f47e99b8)
  and E3-v1 (merge SHA c7915478) also remain
  ESTABLISHED. Re-tag (v2, v3...) only after
  an explicit rollback or post-baseline
  critical fix.
end note

note right of WS7
  CLOSED POSITION (2026-09-03, Elab Iter 7,
  Cycle 1 — record-correction pass): the E7
  workspace traversed every state.
  IntegrationWork DONE with an EMPTY set:
  the pass's work is artifact-record
  corrections owned by other roles (A-42
  Test Manager, A-43 Process Engineer,
  A-44/A-45 System Analyst, A-46 Tester) —
  no code handoff is owed, none entered
  the gate (0 ready-for-review branches,
  0 open PRs at cycle open; tree unchanged
  vs E6-v1). LAMClosePR DONE: PR #15
  (iteration/E7 -> main) opened by the
  Integrator, consolidated review state
  APPROVED (verified via
  scm_get_pull_request_review_state).
  GATE PASSED: review APPROVED + main CI
  green (run 33711675908, re-verified
  post-merge). TAG WRITTEN:
  baseline-elaboration-E7-v1 on merge SHA
  a3b4fc9a82dd033e4d08042ecfca5b75cc48f55a.
end note
@enduml
```

### 5.3 Construction — Feature Branches

- UC realizations on `feature/C{n}-{uc-id}-{subject}` based on `iteration/C{n}`.
- The Code Reviewer reviews each feature PR.
- The Integrator merges APPROVED PRs into `iteration/C{n}`.
- At IOC (end of Construction iteration), the Integrator opens `iteration/C{n} → main`.
- The Architect reviews the iteration-close PR; the merge produces the Construction
  baseline.

### 5.4 Transition — Hotfixes

- `hotfix/{issue-id}` branched from `main`.
- Express review by the Code Reviewer.
- Merged to `main` with a patch baseline tag (`baseline-transition-T{n}-v{x+1}`).

---

## 6. Baseline Pedigree

A baseline tag is written **only** when two gates pass:

1. **Review gate:** `scm_get_pull_request_review_state` on the iteration-close PR
   returns `APPROVED` (Architect has signed off).
2. **CI gate:** `scm_get_build_status("main")` returns `green` **after** the merge.

Either gate fails → the Configuration Manager files an SCM issue
(`severity:blocker`, `nature:defect`) and **does NOT tag**.

```plantuml
@startuml
title Baseline Pedigree State Machine

[*] --> IterationWork

state IterationWork {
    IterationWork : Feature branches developed
    IterationWork : Code Reviewer reviews per-feature PRs
    IterationWork : Integrator merges APPROVED into iteration/{phase}{n}
}

IterationWork --> GateCheck : Integrator opens iteration-close PR

state GateCheck {
    state "Review State Check" as ReviewCheck
    state "CI Status Check" as CICheck

    ReviewCheck : scm_get_pull_request_review_state
    CICheck : scm_get_build_status("main")
}

GateCheck --> TagBaseline : [APPROVED AND CI green]
GateCheck --> EscalateBlocker : [NOT APPROVED OR CI red]

state TagBaseline {
    TagBaseline : scm_create_tag("baseline-{phase}{n}-v1")
    TagBaseline : Tag message = audit record
    TagBaseline : (PR number, SHA, review ID, CI URL)
}

state EscalateBlocker {
    EscalateBlocker : scm_create_issue(severity:blocker, nature:defect)
    EscalateBlocker : DO NOT tag
    EscalateBlocker : Wait for fix → re-check gates
}

TagBaseline --> [*] : Baseline established
EscalateBlocker --> GateCheck : Fix applied, re-verify

note right of TagBaseline
  Re-tag (v2, v3...) only after
  rollback or post-baseline
  critical fix.
end note

note right of EscalateBlocker
  A tag on a red build or
  unreviewed commit is a
  DEFECT, not a baseline.
end note

@enduml
```

---

## 7. Baseline Tag Naming

| Tag Pattern | Phase | Example |
|---|---|---|
| `baseline-elaboration-E{n}-v{x}` | Elaboration | `baseline-elaboration-E1-v1` |
| `baseline-construction-C{n}-v{x}` | Construction | `baseline-construction-C1-v1` |
| `baseline-transition-T{n}-v{x}` | Transition | `baseline-transition-T1-v1` |

- `{n}` = iteration number (integer, starting at 1).
- `{x}` = patch version (integer, starting at 1).
- Re-tag (`v2`, `v3`…) only after an explicit rollback or post-baseline critical fix.
- Normal iteration work targets the **next** iteration's tag, not a re-tag of the previous.

### Tag Message (Audit Record)

Every baseline tag message MUST contain:

- Iteration-close PR number and head commit SHA
- Architect approval review ID
- `main` CI run URL at tag time
- Notable findings (naming violations, deferred items, re-tag justifications)

### 7.1 Baseline Register — Baseline Identification Scheme

The register is the **authoritative identification record** of every baseline tag
(the CM plan's baseline identification scheme, per the Elaboration Iter 1 work order).
One row per tag, appended at iteration close by the Configuration Manager **after**
dual-gate verification. A row is never edited after its tag is written — a re-tag
(`v{x+1}`) appends a NEW row and flips the prior row's status to `SUPERSEDED`, with
the rollback justification recorded in the superseding row.

**Naming reconciliation (updated 2026-09-02, Elab Iter 3):** the `iteration/E1`
workspace closed at the end of **Elaboration Iteration 3** (the convergence cycle
that absorbed the Iteration 1 and Iteration 2 mechanism work after the NO-GO LCA
verdicts). Per the iteration-number rule above, the tag encodes the **closing**
iteration: `baseline-elaboration-E3-v1`. The workspace branch keeps its historical
name `iteration/E1` — branch names are minted at creation and never renamed. The
never-written `baseline-elaboration-E1-v1` and `baseline-elaboration-E2-v1`
anticipations are **withdrawn**: no tag was ever created under either name
(Iterations 1 and 2 closed NO-GO with no LAM-close PR; the dual gate was
unevaluable), so no register row ever recorded an ESTABLISHED tag under those
names — replacing the anticipation rows violates no register discipline.

**E4 position (recorded 2026-09-02, Elab Iter 4):** the `iteration/E4` workspace
closed at the end of **Elaboration Iteration 4** (the record-propagation pass); its
tag is `baseline-elaboration-E4-v1` — branch name and closing iteration coincide,
so no reconciliation is needed. `baseline-elaboration-E3-v1` **remains ESTABLISHED**:
E4-v1 is a distinct iteration baseline that **builds on** E3-v1 (E3 content plus
the E4 R003 state-comment correction), not a re-tag — no SUPERSEDED flip applies.

**E5 position (recorded 2026-09-02, Elab Iter 5):** the `iteration/E5` workspace
closed at the end of **Elaboration Iteration 5** (the record-correction pass); its
tag is `baseline-elaboration-E5-v1` — branch name and closing iteration coincide,
so no reconciliation is needed. `baseline-elaboration-E4-v1` **remains ESTABLISHED**:
E5-v1 is a distinct iteration baseline that **builds on** E4-v1 (E4 content plus
the E5 integration record; the integration set was EMPTY — no code handoff entered
the gate this pass), not a re-tag — no SUPERSEDED flip applies. E3-v1 likewise
remains ESTABLISHED.

**E6 position (recorded 2026-09-03, Elab Iter 6):** the `iteration/E6` workspace
closed at the end of **Elaboration Iteration 6** (the R014 record-correction cycle);
its tag is `baseline-elaboration-E6-v1` — branch name and closing iteration coincide,
so no reconciliation is needed. `baseline-elaboration-E5-v1` **remains ESTABLISHED**:
E6-v1 is a distinct iteration baseline that **builds on** E5-v1 (E5 content plus
the E6 integration record; the integration set was EMPTY — no code handoff entered
the gate this cycle), not a re-tag — no SUPERSEDED flip applies. E4-v1 and E3-v1
likewise remain ESTABLISHED.

**E7 position (recorded 2026-09-03, Elab Iter 7):** the `iteration/E7` workspace
closed at the end of **Elaboration Iteration 7** (the record-correction pass); its
tag is `baseline-elaboration-E7-v1` — branch name and closing iteration coincide,
so no reconciliation is needed. `baseline-elaboration-E6-v1` **remains ESTABLISHED**:
E7-v1 is a distinct iteration baseline that **builds on** E6-v1 (E6 content plus
the E7 integration record; the integration set was EMPTY — no code handoff entered
the gate this pass), not a re-tag — no SUPERSEDED flip applies. E5-v1, E4-v1 and
E3-v1 likewise remain ESTABLISHED.

| Tag | Status | Iteration-close PR | Head SHA | Architect review ID | `main` CI run URL (tag time) | Notable findings |
|---|---|---|---|---|---|---|
| `baseline-elaboration-E3-v1` | **ESTABLISHED** — tag written 2026-09-02 after dual-gate verification | #6 (`iteration/E1 → main`, merged 2026-09-02) | `c79154782f719c3e97b098cf3abd3ea83a3b553b` | PR #6 consolidated review state **APPROVED** (verified via `scm_get_pull_request_review_state`, 2026-09-02); per-mechanism approval chain (Code Reviewer, base `iteration/E1`): PR #3 R001 — review 5088169328, PR #4 R003 — review 5088169517, PR #5 R004 — review 5088169685 | run 33598979875 — https://api.github.com/repos/banense-test/employee-portal/actions/runs/33598979875/logs (completed 2026-09-02 06:29:05Z) | 3 Minor code-review findings open (F-CR-E3-1 interim INT-016 adapter deviation — DEFERRED to Construction Iter 1 per R008; F-CR-E3-2 INT-011 contract-table gap — Designer next pass; F-CR-E3-3 OIDC state-comment overstatement — Implementer next code touch): owned, non-blocking, phase-exit conditions per the stakeholder all-findings directive. SCM Issue #1 (severity:blocker, cr:approved): remediation evidence now in SCM (PRs #3/#4/#5 merged to `iteration/E1`; PR #6 merged to `main`) — CR state transition owned by the CCM. LCA evidence package (TC-001…TC-023 execution + PoC empirical results) remains open work owned by other roles — **this tag freezes the architecture baseline; it does NOT declare the LCA milestone achieved** |
| `baseline-elaboration-E4-v1` | **ESTABLISHED** — tag written 2026-09-02 after dual-gate verification | #8 (`iteration/E4 → main`, merged 2026-09-02) | `f47e99b814fd54a7317dcedbf682e1df8e9395c0` | PR #8 consolidated review state **APPROVED** (verified via `scm_get_pull_request_review_state`, 2026-09-02); correction approval chain (Code Reviewer, base `iteration/E4`): PR #7 R003 state-comment — review 5090059324, CI green run 33632200967 | run 33629662894 — https://api.github.com/repos/banense-test/employee-portal/actions/runs/33629662894/logs (completed 2026-09-02 12:25:01Z; re-verified post-merge) | F-CR-E3-3 RESOLVED in this baseline (PR #7 — all three overstated-CSRF comment locations corrected with the honest [DEFERRED — lands with the session mechanism, Construction] marker). F-CR-E3-1 (interim INT-016 adapter — PG adapter lands Construction Iter 1 per R008) and F-CR-E3-2 (INT-011 contract-table evolution — Designer next pass) remain open, Construction-scope/Designer-owned, non-Elaboration-blocking. Builds on `baseline-elaboration-E3-v1` (merge SHA `c7915478`) — distinct iteration baseline, not a re-tag. The 8 BLOCKED test cases are a recorded SCOPE decision — production AD and Keycloak integration belongs to Construction (R010), deferred, not missing (stakeholder framing directive, Iter 3). **This tag freezes the architecture baseline at E4 close; it does NOT declare the LCA milestone achieved** — phase-level sanction remains withheld per the stakeholder all-findings directive; the fresh sanction request fires at the R6 re-presentation with the evidence package |
| `baseline-elaboration-E5-v1` | **ESTABLISHED** — tag written 2026-09-02 after dual-gate verification | #10 (`iteration/E5 → main`, merged 2026-09-02) | `58484d213fa199dbbd3c99472d6eed548b87e8c6` | PR #10 consolidated review state **APPROVED** (verified via `scm_get_pull_request_review_state`, 2026-09-02); integration set EMPTY this pass — no per-mechanism PR chain (the 3/3 mechanisms remain VERIFIED on main from the E3/E4 approval chain: PR #3 R001 — review 5088169328, PR #4 R003 — review 5088169517, PR #5 R004 — review 5088169685, PR #7 R003 state-comment — review 5090059324) | run 33639518709 — https://api.github.com/repos/banense-test/employee-portal/actions/runs/33639518709/logs (completed 2026-09-02 14:04:14Z; re-verified post-merge) | EMPTY integration set — E5 is the record-correction pass; no code handoff entered the gate (0 ready-for-review branches, 0 open PRs at cycle open; tree unchanged vs E4-v1). Open record-propagation findings are artifact-record corrections owned by other roles (TES#F3 Major — A-37 Test Manager; PoC#F3 Minor — A-38 Software Architect; DC#F4 Minor — A-39 Process Engineer) — none is an SCM defect, none blocks this tag. F-CR-E3-1 (interim INT-016 adapter — PG adapter lands Construction Iter 1 per R008) and F-CR-E3-2 (INT-011 contract-table evolution — Designer next pass) remain open, Construction-scope/Designer-owned, non-Elaboration-blocking. Zero open SCM issues — Issues #1/#2/#9 all closed (cr:complete) on their verified evidence. The 8 BLOCKED test cases are a recorded SCOPE decision — production AD and Keycloak integration belongs to Construction (R010), deferred, not missing (stakeholder framing directive, Iter 3). Builds on `baseline-elaboration-E4-v1` (merge SHA `f47e99b8`) — distinct iteration baseline, not a re-tag. **This tag freezes the architecture baseline at E5 close; it does NOT declare the LCA milestone achieved** — phase-level sanction remains withheld per the stakeholder all-findings directive; the fresh sanction request fires at the R6 re-presentation with the evidence package |
| `baseline-elaboration-E6-v1` | **ESTABLISHED** — tag written 2026-09-03 after dual-gate verification | #13 (`iteration/E6 → main`, merged 2026-09-02) | `264f5fec1fb03569156dc8607acb923ec1b08d01` | PR #13 consolidated review state **APPROVED** (verified via `scm_get_pull_request_review_state`, 2026-09-03); integration set EMPTY this pass — no per-mechanism PR chain (the 3/3 mechanisms remain VERIFIED on main from the E3/E4 approval chain: PR #3 R001 — review 5088169328, PR #4 R003 — review 5088169517, PR #5 R004 — review 5088169685, PR #7 R003 state-comment — review 5090059324) | run 33658332611 — https://api.github.com/repos/banense-test/employee-portal/actions/runs/33658332611/logs (completed 2026-09-02 17:01:01Z; re-verified post-merge) | EMPTY integration set — E6 is the R014 record-correction cycle; no code handoff entered the gate (0 ready-for-review branches, 0 open PRs at cycle open; tree unchanged vs E5-v1). Open record-propagation findings are artifact-record corrections owned by other roles (TES#F4 Major — A-40 Test Manager; DC#F5 Minor — A-41 Process Engineer) — neither is an SCM defect, neither blocks this tag. F-CR-E3-2 (INT-011 contract-table evolution) CLOSED at Iter 6 on first-hand Design Model verification; F-CR-E3-1 (interim INT-016 adapter — PG adapter lands Construction Iter 1 per R008) remains open, Construction-scope, non-Elaboration-blocking. Zero open SCM issues — Issues #1/#2/#9 all closed (cr:complete) on their verified evidence. The 8 BLOCKED test cases are a recorded SCOPE decision — production AD and Keycloak integration belongs to Construction (R010), deferred, not missing (stakeholder framing directive, Iter 3). Builds on `baseline-elaboration-E5-v1` (merge SHA `58484d21`) — distinct iteration baseline, not a re-tag. **This tag freezes the architecture baseline at E6 close; it does NOT declare the LCA milestone achieved** — phase-level sanction remains withheld per the stakeholder all-findings directive; the fresh sanction request fires at the R6 re-presentation with the evidence package |
| `baseline-elaboration-E7-v1` | **ESTABLISHED** — tag written 2026-09-03 after dual-gate verification | #15 (`iteration/E7 → main`, merged 2026-09-03) | `a3b4fc9a82dd033e4d08042ecfca5b75cc48f55a` | PR #15 consolidated review state **APPROVED** (verified via `scm_get_pull_request_review_state`, 2026-09-03); integration set EMPTY this pass — no per-mechanism PR chain (the 3/3 mechanisms remain VERIFIED on main from the E3/E4 approval chain: PR #3 R001 — review 5088169328, PR #4 R003 — review 5088169517, PR #5 R004 — review 5088169685, PR #7 R003 state-comment — review 5090059324) | run 33711675908 — https://api.github.com/repos/banense-test/employee-portal/actions/runs/33711675908/logs (completed 2026-09-03 03:32:43Z; re-verified post-merge) | EMPTY integration set — E7 is the record-correction pass; no code handoff entered the gate (0 ready-for-review branches, 0 open PRs at cycle open; tree unchanged vs E6-v1). Open record-propagation findings are artifact-record corrections owned by other roles (TES#F5 — A-42 Test Manager; DC#F6 — A-43 Process Engineer; UCM#F1 — A-44 System Analyst; SUP#F1 — A-45 System Analyst; TC#F2 — A-46 Tester) — none is an SCM defect, none blocks this tag. F-CR-E3-1 (interim INT-016 adapter — PG adapter lands Construction Iter 1 per R008) remains open, Construction-scope, non-Elaboration-blocking. SCM Issue #14 (cr:approved, assigned:test-manager — CR vehicle for TES F4/A-40, remediation landed and ledger-closed at Iter 6) open pending its cr:complete transition — CCM-owned. The 8 BLOCKED test cases are a recorded SCOPE decision — production AD and Keycloak integration belongs to Construction (R010), deferred, not missing (stakeholder framing directive, Iter 3). Builds on `baseline-elaboration-E6-v1` (merge SHA `264f5ec`) — distinct iteration baseline, not a re-tag. **This tag freezes the architecture baseline at E7 close; it does NOT declare the LCA milestone achieved** — phase-level sanction remains withheld per the stakeholder all-findings directive; the fresh sanction request fires at the R6 re-presentation with the evidence package |

**Status vocabulary:** `PENDING` (iteration in progress; gates not yet evaluable) →
`ESTABLISHED` (tag written on an APPROVED + CI-green commit) → `SUPERSEDED`
(replaced by `v{x+1}` after rollback or post-baseline critical fix — justification
mandatory).

**Register discipline:** the Configuration Manager re-verifies both gates
(`scm_get_pull_request_review_state` + `scm_get_build_status("main")`) immediately
before every `scm_create_tag`, then flips the row to `ESTABLISHED` in the same
commit cycle. A register row claiming `ESTABLISHED` without both gate values
recorded is a defect.

### 7.2 Baseline Identification Content Map — what each baseline freezes

```plantuml
@startuml
title Baseline Identification Scheme — configuration items frozen per baseline family

skinparam componentStyle rectangle

package "baseline-elaboration-E{n}-v{x}\n(architecture baseline)" as ELAB {
  component "SAD 4+1 baseline\n(7 diagrams · 11 COMP · 4 ADR)" as SAD_V
  component "Evolutionary mechanism code (src/)\nR001 LDAP gateway · R003 OIDC client\nR004 offline queue + idempotent sync" as MECH_C
  component "Dual-coverage mechanism tests" as MECH_T
}

package "baseline-construction-C{n}-v{x}\n(iteration baseline)" as CONS {
  component "UC realizations\n(feature/C{n}-{uc-id} merges)" as UC_R
  component "Regression suite green on main" as REG_S
}

package "baseline-transition-T{n}-v{x}\n(release baseline)" as TRANS {
  component "Release candidate on main" as REL_C
  component "hotfix/{issue-id} patches\n(re-tag v{x+1} after critical fix)" as HOT_P
}

component "Baseline Register\nBRANCHING_STRATEGY.md - Baseline Register" as REG

ELAB ..> REG : one row per tag
CONS ..> REG : one row per tag
TRANS ..> REG : one row per tag

note bottom of ELAB
  E7 v1 status: ESTABLISHED — dual gate
  verified 2026-09-03 (iteration-close
  PR #15 APPROVED; main CI green, run
  33711675908); tag written on merge SHA
  a3b4fc9. Builds on E6-v1 (merge SHA
  264f5ec, ESTABLISHED) — the E7
  record-correction pass carried an
  EMPTY integration set (integration
  record only; no code handoff). E5-v1
  (merge SHA 58484d21), E4-v1 (merge
  SHA f47e99b8) and E3-v1 (merge
  SHA c7915478) also remain ESTABLISHED.
  See Baseline Register (§7.1).
end note

note right of REG
  Register row = the audit record:
  Tag | Status (PENDING / ESTABLISHED /
  SUPERSEDED) | Iteration-close PR |
  Head SHA | Architect review ID |
  main CI run URL | Notable findings
end note
@enduml
```

---

## 8. Cross-Phase Invariants

1. **Only the Integrator writes `iteration/*` and `main`.** No other role pushes
   directly to these branches.
2. **`ready-for-review` is the Implementer → Code Reviewer handoff label.** The
   Implementer labels a feature branch `ready-for-review`; the Code Reviewer lists
   branches with that label to find work.
3. **A baseline tag freezes only an APPROVED + CI-green commit.** No exceptions.
4. **No `poc/` branches or `samples/` directories.** All code is evolutionary and
   lives in `src/`.
5. **`docs/BRANCHING_STRATEGY.md` updates go direct to `main`** via
   `scm_commit_files` — no PR, no review label.
6. **CI triggers on all branch families** for both push and PR events.

---

## 9. Change Control Interface

The Change Control Manager (CCM) owns the Change Request state machine
(`cr:new` → `cr:approved` → `cr:complete`). The Configuration Manager does NOT
triage CRs or evaluate impact — that is the CCM's responsibility.

The Configuration Manager consumes CCM-triaged outcomes **indirectly** via the
branches and PRs they authorize:

- A `cr:approved` CR authorizes a feature branch (e.g., `feature/C2-UC-007-browse-news`).
- The Implementer creates the branch and implements the change.
- Normal review + merge flow applies.
- The Configuration Manager verifies naming compliance and gate integrity, not CR
  triage.

---

## 10. Escalation Procedures

| Condition | Action | Issue Labels |
|---|---|---|
| Iteration-close PR not APPROVED | File issue, do NOT tag | `severity:blocker`, `nature:defect` |
| `main` CI red after merge | File issue, do NOT tag | `severity:blocker`, `nature:defect` |
| Branch name violates convention | File issue, do NOT auto-rename | `severity:minor`, `nature:defect`, `naming-violation` |
| Re-tag without rollback justification | Reject re-tag, file issue | `severity:minor`, `nature:defect` |

---

## 11. Tooling

| Tool | Purpose |
|---|---|
| Git (SCM) | Version control, branching, tagging |
| CI/CD pipeline | Build + test on push and PR; status checked before baseline tagging |
| GitHub Issues | Change Requests (CCM), gate-failure escalations (CM), naming violations (CM) |
| Pull Requests | Code review gate; iteration-close PR carries Architect approval |
| Branch labels | `ready-for-review` handoff; cross-role coordination |

---

## 12. Project-Specific Context

- **Deployment:** Custom-built, single-node Windows Server (CON-008). No cloud.
- **Auth:** OIDC via existing Keycloak (CON-004). Portal is a client only.
- **Directory:** Live LDAP read from AD (CON-005, CON-006). No sync, no local copy.
- **DB:** PostgreSQL (CON-003). Stores only `uid → category` (CON-006).
- **Tech stack:** .NET 10 REST API + Razor Pages (CON-001, CON-002).
- **Key risks:** R001 (LDAP attribute gaps — HIGH), R002 (clocking adoption); Risk List
  entries R003 (OIDC), R004 (offline resilience), R010 (STK-004 deliverables).
- **Elaboration mechanism work (stakeholder decision, Elab Iter 1):** the PoC is
  produced in Elaboration and validated **empirically** — R001 against a disposable
  LDAP directory, R003 against a stub OIDC issuer, R004 direct — all evolutionary in
  `src/` on `feature/E1-{risk-id}` branches based on `iteration/E1`.
- **Elaboration test priority:** R001 > R003 > R004; first test cases target UC-001, UC-004, UC-010.
- **Three blocking deployment dependencies on STK-004:** server, LDAP, Keycloak client.
- **Architecture baseline (ESTABLISHED):** `baseline-elaboration-E7-v1` (2026-09-03) —
  the E3 baseline content (SAD 4+1 + R001/R003/R004 evolutionary mechanism code +
  dual-coverage mechanism tests, frozen at `baseline-elaboration-E3-v1`, merge SHA
  `c7915478`) plus the E4 R003 state-comment correction (PR #7, F-CR-E3-3 remediation,
  frozen at `baseline-elaboration-E4-v1`, merge SHA `f47e99b8`) plus the E5 integration
  record (empty integration set — record-correction pass, frozen at
  `baseline-elaboration-E5-v1`, merge SHA `58484d21`) plus the E6 integration record
  (empty integration set — R014 record-correction cycle, frozen at
  `baseline-elaboration-E6-v1`, merge SHA `264f5ec`) plus the E7 integration record
  (empty integration set — record-correction pass), frozen on merge SHA `a3b4fc9`
  after dual-gate verification (PR #15 APPROVED; main CI green run 33711675908).
  Construction feature branches build on this baseline.

---

## Traceability

| Element | Traces From | Link Type | Traces To |
|---|---|---|---|
| BRANCHING_STRATEGY.md | CON-001, CON-002, CON-003, CON-004, CON-005, CON-006, CON-008 | DependsOn | All phase baselines |
| Branch naming conventions | RUP Ch.13 (Manage Baselines and Releases) | Refines | feature/*, iteration/*, hotfix/* branches |
| Baseline tag conventions | RUP Ch.13 | Refines | baseline-elaboration-*, baseline-construction-*, baseline-transition-* |
| Gate verification process | RUP Ch.13 | Refines | scm_get_pull_request_review_state, scm_get_build_status |
| Escalation procedures | RUP Ch.13 (CCB) | DependsOn | scm_create_issue |
| R001 (LDAP risk) | Declared risk | DependsOn | feature/E1-R001-* branch family |
| STK-004 (Infra Team) | Declared stakeholder | DependsOn | Deployment dependencies (server, LDAP, Keycloak client) |
| Baseline Register (§7.1) | RUP Ch.13; Elaboration Iter 1 work order (baseline identification scheme) | Refines | `baseline-elaboration-E3-v1` (ESTABLISHED 2026-09-02 — E1 workspace closed at end of Elab Iter 3); `baseline-elaboration-E4-v1` (ESTABLISHED 2026-09-02 — E4 workspace closed at end of Elab Iter 4); `baseline-elaboration-E5-v1` (ESTABLISHED 2026-09-02 — E5 workspace closed at end of Elab Iter 5); `baseline-elaboration-E6-v1` (ESTABLISHED 2026-09-03 — E6 workspace closed at end of Elab Iter 6); `baseline-elaboration-E7-v1` (ESTABLISHED 2026-09-03 — E7 workspace closed at end of Elab Iter 7); every future baseline tag |
| Baseline Identification Content Map (§7.2) | RUP Ch.13; SAD (4+1 baseline, COMP-001…011, ADR-001…004) | Refines | SAD, mechanism code (`src/`), regression suite, release candidates |
| E1 lifecycle diagram (§5.2) | BRANCHING_STRATEGY §5.2, §6; Review Record F-CR-E1-1 (RESOLVED Iter 3 — 3 branches, 3 PRs, 3 APPROVED); verified SCM state 2026-09-02 | Refines | `iteration/E1 → main` flow (PR #6, merged 2026-09-02); `baseline-elaboration-E3-v1` (tag written 2026-09-02, merge SHA `c7915478`) |
| E4 lifecycle diagram (§5.2) | BRANCHING_STRATEGY §5.2, §6; Review Record Iter 4 code-review-lens record (PR #7 APPROVED review 5090059324; F-CR-E3-3 RESOLVED); verified SCM state 2026-09-02 (PR #8 merged, merge SHA `f47e99b8`) | Refines | `iteration/E4 → main` flow (PR #8, merged 2026-09-02); `baseline-elaboration-E4-v1` (tag written 2026-09-02, merge SHA `f47e99b8`) |
| E5 lifecycle diagram (§5.2) | BRANCHING_STRATEGY §5.2, §6; Review Record Iter 5 code-review-lens record (No-PRs-To-Review — 0 ready-for-review branches, 0 open PRs, main GREEN run 33639518709); verified SCM state 2026-09-02 (PR #10 merged, merge SHA `58484d21`) | Refines | `iteration/E5 → main` flow (PR #10, merged 2026-09-02); `baseline-elaboration-E5-v1` (tag written 2026-09-02, merge SHA `58484d21`) |
| E6 lifecycle diagram (§5.2) | BRANCHING_STRATEGY §5.2, §6; Review Record Iter 6 code-review-lens record (No-PRs-To-Review — 0 ready-for-review branches, 0 open PRs, main GREEN run 33658332611); verified SCM state 2026-09-03 (PR #13 merged, merge SHA `264f5ec`) | Refines | `iteration/E6 → main` flow (PR #13, merged 2026-09-02); `baseline-elaboration-E6-v1` (tag written 2026-09-03, merge SHA `264f5ec`) |
| E7 lifecycle diagram (§5.2) | BRANCHING_STRATEGY §5.2, §6; Elaboration Iter 7 work order (E7 close — PR #15 opened by the Integrator, consolidated review APPROVED); verified SCM state 2026-09-03 (PR #15 merged, merge SHA `a3b4fc9`) | Refines | `iteration/E7 → main` flow (PR #15, merged 2026-09-03); `baseline-elaboration-E7-v1` (tag written 2026-09-03, merge SHA `a3b4fc9`) |
| `baseline-elaboration-E4-v1` register row (§7.1) | RUP Ch.13; dual-gate verification record 2026-09-02 (PR #8 review state APPROVED via scm_get_pull_request_review_state; main CI green run 33629662894, re-verified post-merge) | Refines | Construction entry baseline; every future baseline tag |
| `baseline-elaboration-E5-v1` register row (§7.1) | RUP Ch.13; dual-gate verification record 2026-09-02 (PR #10 review state APPROVED via scm_get_pull_request_review_state; main CI green run 33639518709, re-verified post-merge) | Refines | Construction entry baseline; every future baseline tag |
| `baseline-elaboration-E6-v1` register row (§7.1) | RUP Ch.13; dual-gate verification record 2026-09-03 (PR #13 review state APPROVED via scm_get_pull_request_review_state; main CI green run 33658332611, re-verified post-merge) | Refines | Construction entry baseline; every future baseline tag |
| `baseline-elaboration-E7-v1` register row (§7.1) | RUP Ch.13; dual-gate verification record 2026-09-03 (PR #15 review state APPROVED via scm_get_pull_request_review_state; main CI green run 33711675908, re-verified post-merge) | Refines | Construction entry baseline; every future baseline tag |
| CONTRIBUTING.md (branch-strategy section) | Review Record F-CR-E1-2 / A-5 (CM share — committed, verified 2026-09-02) | Implements | CR-1 citable rule baseline (branch/PR/merge matters) |