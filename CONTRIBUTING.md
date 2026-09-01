# CONTRIBUTING — Employee Portal

**Project:** Employee Portal (Cuba Corp)
**Maintainers:** Software Architect (coding standards) · Implementer (test conventions) · Configuration Manager (branch strategy)
**Status:** Partial — branch-strategy section committed by the Configuration Manager (Elaboration Iter 1, Cycle 1, 2026-09-01). Coding-standards and test-convention sections are pending from their owners (Review Record F-CR-E1-2 / A-5).
**Last Updated:** 2026-09-01

---

## Branch Strategy (owner: Configuration Manager)

This section is the citable rule baseline for code-review checklist item **CR-1** in all branch, PR, and merge matters. The full model lives in `docs/BRANCHING_STRATEGY.md`; this section states the rules every contributor MUST follow.

### 1. Branch naming (mandatory)

| Pattern | Phase | Example |
|---|---|---|
| `feature/E{n}-{risk-id}[-{mechanism}]` | Elaboration | `feature/E1-R001-ldap-attribute-mapping` |
| `iteration/E{n}` | Elaboration | `iteration/E1` |
| `feature/C{n}-{uc-id}-{subject}` | Construction | `feature/C1-UC-004-clock-in-out` |
| `iteration/C{n}` | Construction | `iteration/C1` |
| `hotfix/{issue-id}` | Transition | `hotfix/CR-015` |
| `chore/{subject}` | Any | `chore/ci-config` |

A branch that does not match its phase pattern is a **naming violation** — surfaced as an SCM issue (`severity:minor`, `nature:defect`, `naming-violation`). The Configuration Manager does NOT auto-rename; the branch owner corrects the name.

### 2. Branch write permissions (invariant)

- Only the **Integrator** writes `iteration/*` and `main`. No other role pushes directly to these branches.
- The Implementer works on `feature/*`; the Code Reviewer opens PRs; the Integrator merges APPROVED PRs.

### 3. Handoff protocol

1. Implementer labels the feature branch `ready-for-review`.
2. Code Reviewer lists branches with that label and opens ONE PR per branch (base = the current `iteration/{phase}{n}` — the Reviewer owns the PR and its base).
3. Every PR body carries a **traceability trailer**: `Implements: UC-NNN` or the risk-id it retires (checklist CR-4).
4. CI must be green on the PR head before review proceeds (CR-5 hard gate — CI red ⇒ request_changes, no code review).

### 4. No throwaway code

All code is evolutionary and lives in `src/`. There is **no `poc/` branch and no `samples/` directory**. Elaboration risk-retirement mechanisms are production code, reviewed exactly like Construction features — never rejected as throwaway, never waived through the checklist.

### 5. Baselines

A baseline tag `baseline-{phase}{n}-v{x}` freezes only an APPROVED + CI-green commit on `main`. Contributors never tag; the Configuration Manager tags at iteration close after dual-gate verification (review state APPROVED + post-merge `main` CI green). The authoritative identification record is the **Baseline Register** in `docs/BRANCHING_STRATEGY.md` §7.1.

### 6. Documentation commits

`docs/BRANCHING_STRATEGY.md` and this file's branch-strategy section go **direct to `main`** via commit — no PR. Source code always goes through a PR.

---

## Coding Standards (owner: Software Architect)

*Pending — to be committed by the Software Architect before or together with the first mechanism PR (Review Record F-CR-E1-2 / A-5). Until this section exists, CR-1 findings are limited to rules citable from the SAD layering rule (dependencies point down; interfaces only across package boundaries) and the Design Model contracts.*

## Test Conventions (owner: Implementer)

*Pending — to be committed by the Implementer together with the first mechanism PR. Dual coverage is mandatory per the review checklist (CR-2): black-box contract tests AND white-box path tests (branches, loops, error handlers).*
