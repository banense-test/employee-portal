## Document Control
| Field | Value |
|---|---|
| Phase | Elaboration |
| Status | Draft — Reviewer verdict at the Elaboration Iter 1 LCA review: **Approved, zero findings** (FURPS+ quantified and testable; thresholds tagged with named bases; traceable to declared NFRs/ACs; no gold-plating). Substantive content preserved unchanged through the Iteration 2 convergence cycle |
| Milestone Target | End of Elaboration (LCA) — NOT yet achieved; re-presentation pending convergence-cycle closure (empty findings ledger across all lenses and severities + empirical R001/R003/R004 evidence package) |
| Iteration | 2 (Cycle 1) — convergence cycle. This revision: Document Control metadata only — zero findings and zero CRs target this artifact. Note: the untagged >90% R001 acceptance criterion flagged as Risk List F1 (Reviewer) lives in the Risk List / SAD PoC Plan / Test Evaluation Summary / TC-011 — it is verified ABSENT from this artifact. All FURPS+ sections, quantified thresholds, cross-cutting mechanisms diagram, and traceability are preserved exactly as reviewed |
| Elaboration Changes | **Iter 2 (this revision):** Document Control updated in place to record the convergence-cycle disposition; no substantive change. **Iter 1 (preserved):** Offline clocking [SCOPE_QUESTION] RETIRED — stakeholder decision recorded (Reliability); Data Integrity section added (DAT-001, DAT-002 — implied NFRs); SEC-006/SEC-007 added (role enforcement, own-data-only access — forced by full UC specifications); INT-005 (CSV download) added; Offline Sync Mechanism added to cross-cutting mechanisms diagram; thresholds marked for Requirements Specifier quantification. **RS Iter 1 additions:** Performance and Reliability thresholds quantified with testable criteria (PRF-001/002/003, REL-001/002/003 — per the recorded stakeholder decision delegating threshold quantification to RS); SYNC mechanism note updated to reference quantified thresholds. **RS Iter 1 post-answer:** timestamp [SCOPE_QUESTION] RETIRED — stakeholder decision recorded: store UTC, display office local timezone, export ISO-8601 with explicit offset, payroll day = local calendar day; stakeholder correction applied — the declared input names no office locations (Havana/Madrid framing withdrawn); all 3 offices share one timezone. **RS Iter 1 final:** office local timezone [SCOPE_QUESTION] RETIRED — stakeholder decision recorded: America/Havana, an IANA identifier, not a fixed offset (Cuba observes DST; a hardcoded UTC-5 would silently shift every payroll day boundary when the clocks change) |
## Functionality

### Security

| ID | Requirement | Source | Volatility |
|---|---|---|---|
| SEC-001 | OIDC authentication via existing Keycloak — portal is a client only; no deployment or realm design | CON-004 | Low |
| SEC-002 | Role-based access control: Employee role (clocking, news browse, directory search) vs HR Administrator role (clocking review, export, category assignment, news management) — roles read from Keycloak claims | CON-004, FR-004 | Low |
| SEC-003 | No anonymous access — all pages require authentication | CON-009 | Low |
| SEC-004 | Internal corporate network only — no external access | CON-009 | Low |
| SEC-005 | No writing back to Active Directory — all AD access is read-only | CON-007 | Low |
| SEC-006 | HR-only use cases (UC-005–UC-010) enforce the HR Administrator role from Keycloak claims; a session holding only the Employee role is rejected from those functions | CON-004, FR-001, FR-002, FR-003, FR-006, FR-008, FR-009 | Low |
| SEC-007 | An employee can access only their own clocking history (UC-002); access to another employee's clocking data requires the HR Administrator role (UC-005) | FR-005, FR-001 | Low |

### Licensing

| ID | Requirement | Source | Volatility |
|---|---|---|---|
| LIC-001 | All infrastructure is open-source or already owned (.NET 10, PostgreSQL, Keycloak) — no additional licensing required | CON-001, CON-003, CON-004 | Low |

### Audit (Cross-Cutting Mechanism — NOT a Use Case)

| ID | Requirement | Source | Volatility |
|---|---|---|---|
| AUD-001 | News publication audited: author + timestamp recorded | NFR-005, FR-006 | Low |
| AUD-002 | News edit audited: editor + timestamp recorded for every edit version | NFR-005, FR-008 | Low |
| AUD-003 | News unpublish audited: actor + timestamp recorded | NFR-005, FR-009 | Low |
| AUD-004 | Worker category change audited: actor + timestamp + old value + new value recorded | NFR-005, FR-003 | Low |
| AUD-005 | Employee directory fields are read-only from AD — no audit needed for directory data | NFR-005 | Low |

**Note:** Audit is a cross-cutting mechanism implemented via `<<include>>` from UC-007, UC-008, UC-009, UC-010. It is NOT a standalone use case.

### Data Integrity (implied NFRs — what stakeholders would reject even if functional requirements were met)

| ID | Requirement | Source | Volatility |
|---|---|---|---|
| DAT-001 | Clocking timestamps are recorded by the system at the moment of the button press; the employee never enters or edits a time | FR-004 | Low |
| DAT-002 | Audit entries are append-only — no portal function modifies or deletes an audit entry (a mutable trail would not satisfy NFR-005's mandatory traceability) | NFR-005 | Low |

## Usability
| ID | Requirement | Source | Volatility |
|---|---|---|---|
| USA-001 | Custom UI design (docs/inputs/employee-portal-design.html) is mandatory and authoritative for the UI visual layer | CON-011 | Low |
| USA-002 | Employee can clock in/out without help from HR or development team | AC-001 | Low |
| USA-003 | Employee finds colleague's phone/email in under 10 seconds | AC-003 | Low |
| USA-004 | 80% of employees complete at least one clocking with no prior training | AC-004 | Low |
| USA-005 | HR Administrator can publish a news item without technical assistance | AC-002 | Low |
| USA-006 | Compatible with current Chrome and Edge browsers | CON-010 | Low |
| USA-007 | Responsive web design (no native mobile app) — works on corporate browsers including mobile viewport | Scope Statement | Low |
| USA-008 | All clocking times are DISPLAYED in the office local timezone (America/Havana, IANA identifier, DST-aware) — never as raw UTC or server time. This is the display facet of the timestamp convention recorded under Reliability (store UTC, display office local, export ISO-8601 with explicit offset) | Stakeholder decision (Elaboration Iter 1 — see Reliability "Timestamp convention"); DAT-001 | Low |
| USA-009 | The mandatory design reference's accessibility rules apply to every screen: AA contrast, visible focus indicators, interactive targets ≥ 40 px, full keyboard operability | CON-011 (design reference accessibility declaration) | Low |

### Quantified Thresholds (User-Interface Designer, Elaboration Iter 1)

Measurable criteria per user role. Each criterion is testable; none invents targets beyond declared sources (gold-plating guard).

| ID | Quantified Threshold | Testable Criteria | Basis |
|---|---|---|---|
| USA-001 | Every screen implements the design reference tokens: palette (brand-900 #0B3D5C, brand-500 #1E7FB5, accent #17A398, danger #C0392B, warn #E6A817), type scale 12/14/16/20/28 px, radius 8 px cards / 6 px controls / 999 px chips, 1120 px container; the clocking button toggles green "Clock In" (accent) ↔ red "Clock Out" (danger) by current status | Side-by-side inspection of every screen against docs/inputs/employee-portal-design.html; button color and label match the employee's clocking status in every state | CON-011 (design tokens declared in the reference header) |
| USA-002 | A clock in/out completes in ≤ 2 interactions from Home (open Home → press the status-aware button); the inline confirmation with the recorded time is the success signal | Unaided user test: employee completes clocking with zero assistance; confirmation visible < 1 s from press (PRF-002) | AC-001; UC-001 steps 4–8 |
| USA-003 | From Directory page load, a colleague's email AND extension are visible on the result card — no detail view needed; total task ≤ 10 s including typing | Timed task: locate a named colleague's email/extension in ≤ 10 s (AC-003); person card renders all six corporate fields (name, job title, department, office, email, extension) | AC-003; PRF-003; design reference person card (CON-011) |
| USA-004 | The Home clocking card is the single primary affordance for first use: one status-aware button, no training material required | First-use test without training: ≥ 80% of sampled employees complete ≥ 1 clocking unaided | AC-004; R002 (adoption risk) |
| USA-005 | Publish completes in one form (title, body, date, category, featured flag) with inline validation feedback on errors | Unaided HR task completion: publish a news item without technical assistance; invalid fields highlighted inline on submit (UC-008 AF-1) | AC-002; UC-008 steps 3–8 |
| USA-006 | Every screen renders and operates correctly in current Chrome and Edge | Manual pass of all screens and interactions in both browsers; no browser-specific breakage | CON-010 |
| USA-007 | Layout collapses to a single column below 900 px viewport (design reference media query); all actions remain operable at mobile width | Walkthrough at 375 px viewport: sidebar, clocking button, search, chips, tables usable | Scope Statement; design reference @media(max-width:900px) (CON-011) |
| USA-008 | Every displayed clocking time (status chip, confirmation, history tables, HR report) renders in America/Havana local time, DST-aware | Inspect displayed values across a DST boundary: displayed local time matches the IANA zone in force at the event time; no raw UTC or server time shown to users | Stakeholder decision (Elaboration Iter 1); DAT-001 |
| USA-009 | AA contrast, visible focus, targets ≥ 40 px, keyboard operable on every interactive element of every screen | Keyboard-only walkthrough of every screen: all actions reachable, focus visible on every control, interactive targets ≥ 40 px (clocking button 52 px per design reference) | CON-011 (design reference accessibility line) |

**UI design realization:** the interaction flows that satisfy these criteria are specified as per-UC storyboards in the Use-Case Model §Use-Case Specifications (UI Flow References) and formalized as boundary classes + navigation map + UI patterns in the Design Model §Boundary Classes and Navigation Map (User-Interface Designer, Elaboration Iter 1).
## Reliability
| ID | Requirement | Source | Volatility |
|---|---|---|---|
| REL-001 | System available during extended working hours: Monday–Friday 7:00–19:00. 24/7 not required. | NFR-003 | Low |
| REL-002 | Fault tolerance within corporate network: system tolerates temporary network disruptions and recovers/syncs data once connectivity is restored | NFR-004 | Medium |
| REL-003 | If network drops for 5 minutes, data syncs once connectivity is restored | AC-005 | Medium |
| REL-004 | Backup and server crash recovery are Infrastructure's responsibility, not a portal requirement | CON-014 | Low |

**Stakeholder decision recorded (Elaboration — [SCOPE_QUESTION] retired):** The offline clocking persistence mechanism is **architectural and within declared scope**. The stakeholder was asked whether NFR-004 and AC-005 (system tolerates a 5-minute network drop; data syncs once connectivity is restored) are met by an architect-owned local-queuing mechanism, and answered **"Yes"**. Division of responsibility: the Software Architect designs the mechanism (Offline Sync Mechanism, R004 PoC in Elaboration Iteration 1); the Requirements Specifier quantifies the thresholds (max queue size, sync timeout, conflict policy); the Use-Case Model captures the observable behavior in UC-001 AF-1. No open question remains on this item.

### Quantified Thresholds (Requirements Specifier, Elaboration Iter 1 — per the recorded stakeholder decision delegating threshold quantification to this role)

| ID | Quantified Threshold | Testable Criteria | Basis |
|---|---|---|---|
| REL-001 | Availability window Mon–Fri 7:00–19:00; during the window the portal serves PRF-001 page loads and PRF-002 clocking responses | During the declared window, pages load and clocking responds per PRF-001/PRF-002; outside the window no availability commitment is made | NFR-003 (declared window); no availability percentage declared — none invented (gold-plating guard) |
| REL-002 | Offline tolerance: network disruptions of at least 5 minutes are tolerated; per-client queue capacity ≥ 10 clocking events per employee browser; queued events are never lost; idempotent conflict policy — an exact duplicate (same employee, same event type, same recorded timestamp) is rejected, never duplicated; events ordered by recorded timestamp, not arrival order | Simulate a 5-minute outage: events queue locally with confirmation shown from queued data; after restore, all queued events are persisted with zero duplicates and zero losses | AC-005 (declared 5 min); queue capacity [ASSUMPTION — requires validation] — at most 2 transitions per employee per workday (FR-004 in/out), so 10 events covers 5 full workdays of total outage; conflict policy per DAT-001 |
| REL-003 | Sync completion: all queued events persisted ≤ 60 seconds after connectivity is restored | After restore, measure time from reconnection to full persistence of the queued set: ≤ 60 s | [ASSUMPTION — requires validation] — worst case 200 employees × 1 queued event each (STK-003 population), small records, restored corporate LAN |
| Timestamp convention | Clocking timestamps are stored in UTC, displayed in the office local timezone (America/Havana, IANA), and exported in ISO-8601 with an explicit offset; the payroll day is the local calendar day, never the server's. All 3 offices share this one timezone (stakeholder-confirmed). | Verify a recorded event end-to-end: stored value is UTC; displayed value is America/Havana local time (DST-aware); exported value carries the explicit offset in force at the event time; month-boundary grouping follows the local calendar day in America/Havana | DAT-001; stakeholder decisions (Elaboration Iter 1): convention + office local zone America/Havana (IANA identifier) |

**Stakeholder decision recorded (Elaboration Iter 1 — timestamp [SCOPE_QUESTION] retired):** The timezone convention for clocking timestamps is decided: **store every clocking timestamp in UTC, display it in the office local timezone, and export ISO-8601 with an explicit offset. The payroll day is the local calendar day, never the server's.** The stakeholder also corrected the record: the declared input names no office locations — the Havana/Madrid framing in the earlier question was invented, not declared — and **all 3 offices are in the same timezone**. The prior working assumption (portal server timezone everywhere) is withdrawn: it only works while there is one timezone and breaks silently the day there isn't. No open question remains on the convention itself.

**Stakeholder decision recorded (Elaboration Iter 1 — office local timezone [SCOPE_QUESTION] retired):** The office local timezone is **America/Havana**. The stakeholder specified an IANA identifier, not a fixed offset — Cuba observes DST, so a hardcoded UTC-5 would be wrong for part of the year and would silently shift every payroll day boundary when the clocks change. The zone completes the decided convention (store UTC, display office local, export ISO-8601 with explicit offset, payroll day = local calendar day); all 3 offices share this one timezone (stakeholder-confirmed). The exported offset is the one in force at each event's time per the IANA zone database. No open question remains on the timestamp convention.

### R001 Behavioural Bar — LDAP attribute absence (stakeholder decision, Elaboration Iter 2)

**Stakeholder decision recorded (Elaboration Iter 2 — R001 validation bar):** The R001 validation bar is **behavioural, not statistical**. The prior ">90% of sampled users per office with all six corporate attributes populated" figure is **dropped** — it is invented (the declared R001 names no percentage; the PoC decision names none), and measured against a disposable directory the team seeds itself it cannot fail, so it proves nothing. The architectural risk is what the portal DOES when an attribute is absent, not how many attributes are missing (a property of the real directory nobody can know until STK-004 delivers). **The bar, in the stakeholder's words:** "every employee is rendered whether or not their attributes are complete; a missing attribute never removes someone from search results; a missing attribute never raises an error. Seed the gaps deliberately and prove those three hold. That retires R001 empirically, this phase, without the production directory." The percentage belongs to a different activity — measuring the real AD's data quality once STK-004 delivers — tracked in Construction (R011 residual), kept out of the LCA evidence package.

**Requirements consequence (Requirements Specifier, Elaboration Iter 2):** the bar is a **reliability contract at the LDAP Query Mechanism boundary** — one contract, four consumers. UC-004 (FR-010) is the declared home of the bar (the stakeholder's wording names search results). UC-005 (FR-001), UC-006 (FR-002), and UC-007 (FR-003) read the same AD attributes through the same mechanism, so the same three clauses are specified there as AF-3 alternative flows, each marked **[DERIVED — from FR-00N + the R001 behavioural bar, awaiting stakeholder confirmation]** — the bar's reach beyond the directory wording is the stakeholder's to confirm, not this role's to assume. The three clauses are testable as written: seed gaps deliberately in the disposable directory, exercise each consumer, and verify (a) rendering completeness, (b) no removal, (c) no error. Distinct-condition note: AD-unreachable (UC-005 AF-2, UC-006 AF-2, UC-007 AF-2, UC-004 AF-3) is a different failure mode with a different contract — it is NOT waived by the behavioural bar.

```plantuml
@startuml
skinparam componentStyle rectangle
skinparam fontSize 11
title R001 Behavioural Bar - One Contract, Four AD-Reading Use Cases\nLDAP Query Mechanism consumers (Elaboration Iter 2)

package "Employee Portal" {
  component "UC-004 Directory Search\n(FR-010 - declared home of the bar)" as UC004
  component "UC-005 HR Clocking Review\n(FR-001 - AF-3 [DERIVED])" as UC005
  component "UC-006 CSV Export\n(FR-002 - AF-3 [DERIVED])" as UC006
  component "UC-007 Category Assignment\n(FR-003 - AF-3 [DERIVED])" as UC007

  component "LDAP Query Mechanism\n(cross-cutting, <<include>>\nfrom UC-004/005/006/007)" as LDAP <<cross-cutting>>
}

component "Active Directory (LDAP)" as AD <<external>>
component "Disposable LDAP Directory\n(deliberately seeded gaps -\nR001 PoC, Elaboration)" as DISPOSABLE <<external>>

LDAP ..> AD : production queries\n(read-only - CON-007)
LDAP ..> DISPOSABLE : PoC validation queries\n(gaps seeded deliberately)

UC004 ..> LDAP
UC005 ..> LDAP
UC006 ..> LDAP
UC007 ..> LDAP

note bottom of LDAP
  One behavioural contract at the
  mechanism boundary (R001 bar,
  stakeholder decision, Elab Iter 2):
  (a) every employee is rendered
  whether or not attributes complete
  (b) a missing attribute never
  removes someone from results
  (c) a missing attribute never
  raises an error
  UC-004: declared home (FR-010).
  UC-005/006/007: [DERIVED - from
  FR-001/FR-002/FR-003 + the bar,
  awaiting stakeholder confirmation]
end note

note bottom of DISPOSABLE
  The >90% statistical criterion is
  DROPPED (invented, unsourceable;
  measured against self-seeded data
  it cannot fail, so it proves nothing).
  Real-AD data-quality measurement
  moves to Construction (R011 residual,
  STK-004-dependent), excluded from
  the LCA evidence package.
end note
@enduml
```
## Performance

| ID | Requirement | Source | Volatility |
|---|---|---|---|
| PRF-001 | Pages must load in under 3 seconds on the corporate network | NFR-001 | Low |
| PRF-002 | Clock in/out operation must respond in under 1 second | NFR-002 | Low |
| PRF-003 | Directory search returns results fast enough for AC-003 (under 10 seconds total including user interaction) | AC-003, FR-010 | Medium |

**Note:** The Requirements Specifier quantifies exact LDAP query timeout thresholds in Elaboration. R001 (LDAP attribute inconsistency) may affect perceived performance if fallback strategies are needed; R005 (LDAP performance) is monitored during the R001 PoC. No further performance targets are declared — inventing ones would be gold-plating.

### Quantified Thresholds (Requirements Specifier, Elaboration Iter 1)

| ID | Quantified Threshold | Testable Criteria | Basis |
|---|---|---|---|
| PRF-001 | Page load < 3 s at the 95th percentile under normal working-hours load (Mon–Fri 7:00–19:00, the REL-001 window) on the corporate network | Automated timing of page loads during the REL-001 window: ≥ 95% of loads complete in < 3 s | NFR-001 (declared 3 s); percentile basis [ASSUMPTION — requires validation] — standard intranet measurement; no load model was declared |
| PRF-002 | Clock in/out confirmation displayed < 1 s from button press — on BOTH the online path and the offline-queued path (UC-001 AF-1) | Measure button-press → confirmation latency with the portal reachable and with the portal server unreachable; both < 1 s | NFR-002 (declared 1 s); the offline path is included because the confirmation is shown from queued data (UC-001 step 8) |
| PRF-003 | Directory search: LDAP query hard timeout of 5 s; end-to-end search (criteria entry → results displayed) ≤ 10 s | Simulated search: LDAP query aborts at 5 s and UC-004 AF-3 "Directory temporarily unavailable" is shown; a typical search completes end-to-end ≤ 10 s | AC-003 (declared 10 s total); the 5 s query split [ASSUMPTION — requires validation] — leaves ≥ 5 s of the AC-003 budget for typing and rendering |

## Supportability

| ID | Requirement | Source | Volatility |
|---|---|---|---|
| SUP-001 | Backend: .NET 10 with REST API — standard framework, maintainable by .NET developers | CON-001 | Low |
| SUP-002 | Frontend: Razor Pages — server-rendered, no SPA complexity | CON-002 | Low |
| SUP-003 | Database: PostgreSQL — standard relational database | CON-003 | Low |
| SUP-004 | Worker category list is externally configured (not managed in the portal) — changes to categories do not require code deployment | CON-013 | Medium |
| SUP-005 | No local copy of employee data — no sync job, no reconciliation, no conflict resolution to maintain | CON-006 | Low |

## Design Constraints

| ID | Constraint | Source |
|---|---|---|
| DC-001 | Backend: .NET 10 with REST API | CON-001 |
| DC-002 | Frontend: Razor Pages (intranet, no SPA) | CON-002 |
| DC-003 | Database: PostgreSQL | CON-003 |
| DC-004 | Hosting: internal Windows Server (no cloud) | CON-008 |
| DC-005 | Keycloak is external — portal is OIDC client only, no deployment/provisioning/realm design | CON-004 |
| DC-006 | AD data read via LDAP on demand — no local copy, no sync | CON-005, CON-006 |
| DC-007 | No write-back to AD | CON-007 |
| DC-008 | Custom UI design is mandatory and authoritative | CON-011 |
| DC-009 | No hard delete of news items — unpublish only | CON-012 |
| DC-010 | Worker category list is fixed, externally configured | CON-013 |

## Interfaces

| ID | Interface | Type | Direction | Source |
|---|---|---|---|---|
| INT-001 | Keycloak OIDC | Authentication/Authorization | Portal → Keycloak (outbound) | CON-004 |
| INT-002 | Active Directory LDAP | Directory query (read-only) | Portal → AD (outbound) | CON-005 |
| INT-003 | PostgreSQL | Data persistence | Portal → PostgreSQL (outbound) | CON-003 |
| INT-004 | Chrome / Edge browsers | User interface | Browser → Portal (inbound) | CON-010 |
| INT-005 | CSV report file download | Data export (monthly clocking report) | Portal → HR Administrator (outbound) | FR-002 |

## Applicable Standards

| ID | Standard | Source | Applicability |
|---|---|---|---|
| STD-001 | OIDC (OpenID Connect) protocol | CON-004 | Authentication interface with Keycloak |
| STD-002 | LDAP v3 | CON-005 | Directory queries to Active Directory |
| STD-003 | CSV format | FR-002 | Clocking report export |
| STD-004 | REST API conventions | CON-001 | Backend API design |
| STD-005 | HTML5 / CSS3 / JavaScript (Razor Pages) | CON-002, CON-010 | Frontend rendering |

## Cross-Cutting Mechanisms Diagram

```plantuml
@startuml
skinparam componentStyle rectangle
skinparam fontSize 11
title Cross-Cutting Mechanisms - Constraints on All Use-Case Realizations

package "Employee Portal" {
  component "Clocking Module" as CLK
  component "News Module" as NEWS
  component "Directory Module" as DIR
  component "Category Module" as CAT

  component "OIDC Auth Mechanism\n(<<include>> from all UCs)" as AUTH <<cross-cutting>>
  component "Audit Trail Mechanism\n(<<include>> from UC-007, 008, 009, 010)" as AUDIT <<cross-cutting>>
  component "LDAP Query Mechanism\n(<<include>> from UC-004, 005, 006, 007)" as LDAP <<cross-cutting>>
  component "Offline Sync Mechanism\n(<<include>> from UC-001)" as SYNC <<cross-cutting>>
}

component "Keycloak (OIDC)" as KC <<external>>
component "Active Directory (LDAP)" as AD <<external>>
database "PostgreSQL" as PG <<external>>

AUTH ..> KC : validates token, reads roles
LDAP ..> AD : read-only queries
CLK --> PG : persists clockings
NEWS --> PG : persists news + audit entries
CAT --> PG : persists uid -> category
AUDIT --> PG : persists audit entries
SYNC --> PG : syncs queued clockings
DIR ..> LDAP : delegates directory queries
CLK ..> LDAP : employee display data (UC-005, UC-006)
CAT ..> LDAP : employee display data (UC-007)

CLK ..> AUTH
NEWS ..> AUTH
DIR ..> AUTH
CAT ..> AUTH
CLK ..> SYNC
NEWS ..> AUDIT
CAT ..> AUDIT

note bottom of SYNC
  Cross-cutting: NOT a use case.
  Stakeholder decision recorded: the
  mechanism is architectural, within
  declared scope (NFR-004, AC-005).
  Design: Software Architect (R004 PoC).
  Thresholds: quantified - see
  Reliability (REL-002, REL-003).
end note

note bottom of LDAP
  Cross-cutting: NOT a use case.
  Read-only (CON-007). No local copy
  (CON-006). R001: attribute
  consistency PoC in Elaboration.
end note

note bottom of AUTH
  Cross-cutting: NOT a use case.
  Keycloak external (CON-004).
  R003: OIDC integration PoC.
end note
@enduml
```

## Traceability

| Element | Traces From | Link Type | Traces To |
|---|---|---|---|
| SEC-001 | CON-004 | Refines | All UCs (<<include>>) |
| SEC-002 | CON-004, FR-004 | Refines | UC-001–UC-010 |
| SEC-005 | CON-007 | Refines | UC-004, UC-005, UC-006, UC-007 |
| SEC-006 | CON-004, FR-001, FR-002, FR-003, FR-006, FR-008, FR-009 | Refines | UC-005, UC-006, UC-007, UC-008, UC-009, UC-010 |
| SEC-007 | FR-005, FR-001 | Refines | UC-002, UC-005 |
| AUD-001 | NFR-005, FR-006 | Refines | UC-008 |
| AUD-002 | NFR-005, FR-008 | Refines | UC-009 |
| AUD-003 | NFR-005, FR-009 | Refines | UC-010 |
| AUD-004 | NFR-005, FR-003 | Refines | UC-007 |
| DAT-001 | FR-004 | Refines | UC-001 |
| DAT-002 | NFR-005 | Refines | UC-007, UC-008, UC-009, UC-010 |
| USA-001 | CON-011 | Refines | All UCs |
| USA-002 | AC-001 | Refines | UC-001 |
| USA-003 | AC-003 | Refines | UC-004 |
| USA-004 | AC-004 | Refines | UC-001 |
| USA-005 | AC-002 | Refines | UC-008 |
| REL-001 | NFR-003 | Refines | (All UCs) |
| REL-002 | NFR-004 | Refines | UC-001 (Offline Sync Mechanism — Architect, R004 PoC) |
| REL-003 | AC-005 | Refines | UC-001 AF-1 (Offline Sync Mechanism — Architect, R004 PoC) |
| REL-002/REL-003 decision | NFR-004, AC-005 + stakeholder answer "Yes" (Elaboration) | Authorizes | Offline Sync Mechanism design (Software Architect) |
| PRF-001 | NFR-001 | Refines | (All UCs) |
| PRF-002 | NFR-002 | Refines | UC-001 |
| PRF-003 | AC-003, FR-010 | Refines | UC-004 |
| PRF-001 quantification | NFR-001 | Refines | All UCs (page loads, REL-001 window) |
| PRF-002 quantification | NFR-002 | Refines | UC-001 (online path + AF-1 offline path) |
| PRF-003 quantification | AC-003, FR-010 | Refines | UC-004 (AF-3 LDAP timeout) |
| REL-002 quantification | NFR-004, AC-005, DAT-001 | Refines | UC-001 AF-1 (Offline Sync Mechanism — Architect, R004 PoC) |
| REL-003 quantification | AC-005 | Refines | UC-001 AF-1 |
| Timestamp convention (stakeholder decision) | DAT-001 + stakeholder answer (Elaboration Iter 1): store UTC, display office local timezone, export ISO-8601 with explicit offset, payroll day = local calendar day | Authorizes | UC-001 timestamp convention; UC-006 CSV column set (event_timestamp) |
| Office local timezone (stakeholder decision) | Stakeholder answer (Elaboration Iter 1): "America/Havana" — an IANA identifier, not a fixed offset; Cuba observes DST, so a hardcoded UTC-5 would silently shift every payroll day boundary when the clocks change | Authorizes | UC-001 timestamp convention; UC-006 event_timestamp column (DST-aware offset per IANA zone database) |
| DC-001 | CON-001 | Refines | (Architecture) |
| DC-002 | CON-002 | Refines | (Architecture) |
| DC-003 | CON-003 | Refines | (Architecture) |
| DC-005 | CON-004 | Refines | (Architecture) |
| DC-006 | CON-005, CON-006 | Refines | UC-004, UC-005, UC-006, UC-007 |
| INT-001 | CON-004 | Refines | (Architecture) |
| INT-002 | CON-005 | Refines | UC-004, UC-005, UC-006, UC-007 |
| INT-005 | FR-002 | Refines | UC-006 |