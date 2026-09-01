# Employee Portal — Deployment Strategy

**Phase:** Inception (Iteration 1, Cycle 1)
**Date:** 2026-09-01
**Owner:** Deployment Manager

## Deployment Mode

**Custom-built** — The Employee Portal is a purpose-built internal web application for Cuba Corp (200 employees, 3 offices). It is not shrink-wrapped or downloadable; it is deployed to a single internal Windows Server operated by the Infrastructure Team (STK-004).

## Target User Community

| Community | Size | Access | Roles |
|---|---|---|---|
| Employees | ~200 across 3 offices | Corporate network, Chrome/Edge browser | Clock in/out, view history, browse news, search directory |
| HR Administrators | Small subset (HR staff) | Corporate network, Chrome/Edge browser | Review clockings, export CSV, assign categories, publish/edit/unpublish news |

## Target Environment

| Attribute | Value | Source |
|---|---|---|
| Hosting | Internal Windows Server (no cloud) | CON-008 |
| Network access | Internal corporate network only — no external access | CON-009 |
| Browsers | Current Chrome and Edge | CON-010 |
| Database | PostgreSQL (co-located on same server) | CON-003 |
| Authentication | Keycloak OIDC (existing, external) | CON-004 |
| Directory | Active Directory LDAP v3 (existing, external, read-only) | CON-005 |
| Backup/Recovery | Infrastructure's responsibility (STK-004) | CON-014 |

## Initial Deployment Topology

Single-node topology on an internal Windows Server within the corporate network:

- **Windows Server (CON-008):** Hosts the .NET 10 Razor Pages application and PostgreSQL database
- **Employee workstations (3 offices):** Access the portal via HTTPS over the corporate network using Chrome or Edge
- **Keycloak (external, CON-004):** OIDC authentication — portal registers as a client only
- **Active Directory (external, CON-005):** LDAP v3 read-only queries for directory data

No load balancers, no clusters, no container orchestration — proportional to 200 users on a single server.

## Rollout Approach

Phased internal deployment:

1. **Development site (Elaboration/Construction):** Portal deployed on a dev Windows Server for integration testing and PoC validation
2. **Acceptance gate 1 — Development site:** Functional acceptance by dev team; all 10 UCs verified against AC-001 through AC-005
3. **Pilot deployment (early Transition):** Portal deployed to production Windows Server; small pilot group (HR + subset from one office)
4. **Acceptance gate 2 — Production site:** Stakeholder acceptance (STK-001) confirms system meets acceptance criteria
5. **Full rollout (Transition):** All 200 employees across 3 offices gain access; adoption tracked against BG-003 (80% within 3 months)

## Rollback Criteria

| Criterion | Trigger | Action |
|---|---|---|
| Authentication failure | OIDC broken — no users can log in | Roll back; verify Keycloak client config with STK-004 |
| LDAP connectivity failure | Directory search errors for all queries | Roll back; verify LDAP service account with STK-004 |
| Data corruption | PostgreSQL integrity issues | Roll back; restore from Infra-managed backup (CON-014) |
| Performance regression | Page load >5s (above NFR-001's 3s) | Roll back; investigate server resources |
| Clocking data loss | Offline sync fails to persist events | Roll back; investigate idempotent endpoint + queue (COMP-009) |

## External Dependencies

| Dependency | Provider | Blocking? |
|---|---|---|
| Windows Server provisioned | STK-004 (Infra) | Yes |
| LDAP service account (read access) | STK-004 (Infra) | Yes |
| Keycloak OIDC client registered | STK-004 (Infra) | Yes |
| PostgreSQL installed on server | STK-004 (Infra) or dev team | Yes |

## Deployment Constraints and Risks

- **CON-008:** Single Windows Server, no cloud — STK-004 provisions and operates
- **CON-009:** Internal network only — no external access config needed
- **CON-014:** Backup/recovery is Infrastructure's responsibility — not in deployment plan
- **R001:** AD LDAP attribute consistency — may require graceful degradation; PoC in Elaboration
- **R002:** Clocking adoption risk — rollout must include communication/training (BG-003)
- **STK-004 dependency:** Server, LDAP, and Keycloak client all blocking; engage early in Elaboration

## Phase Evolution

- **Inception (this document):** Strategy, mode, topology sketch, rollout approach, rollback criteria
- **Elaboration:** Topology refined against SAD; PoC validation of LDAP, OIDC, offline resilience
- **Construction:** Build artifacts packaged; dev-site deployment and acceptance gate 1
- **Transition:** Release Notes drafted; pilot deployment, acceptance gate 2, full rollout, adoption tracking