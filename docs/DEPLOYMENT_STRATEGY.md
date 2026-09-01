# Employee Portal — Deployment Strategy

**Phase:** Inception (Iteration 2)
**Status:** Baseline
**Deployment Manager:** Deployment discipline

## 1. Deployment Mode

**Custom-built** — the Employee Portal is an internal web application deployed as a single-node system on an internal Windows Server within the Cuba Corp corporate network. No cloud, no external access, no distribution media.

**Rationale:** The portal serves a single organization (200 employees, 3 offices). A custom-built deployment on internal infrastructure is proportional to the declared scope (CON-008, CON-009). Shrink-wrapped and downloadable modes do not apply.

## 2. Target User Community

| Group | Size | Role | Access |
|---|---|---|---|
| Employees (STK-003) | 200 | Clock in/out, browse news, search directory | Corporate network, Chrome/Edge |
| HR Administrators (STK-001) | ~5 | Publish/edit/unpublish news, review clockings, export CSV, assign categories | Corporate network, Chrome/Edge |

All users authenticate via Keycloak OIDC (CON-004) with corporate credentials.

## 3. Target Topology

Single-node deployment on Windows Server:

- **Application:** .NET 10 Razor Pages (CON-001, CON-002)
- **Database:** PostgreSQL (CON-003)
- **Authentication:** Keycloak OIDC (CON-004) — external, operated by STK-004
- **Directory:** Active Directory LDAP read-only (CON-005) — external, operated by STK-004
- **Network:** Internal corporate network only (CON-009)
- **Browsers:** Chrome and Edge (CON-010)

No clustering, no load balancers, no container orchestration — proportional to 200-user scope.

## 4. Rollout Approach — Two-Gate Acceptance

### Gate 1: Development Site Acceptance
- Portal deployed to development Windows Server instance
- Verification: all 10 UCs functional, OIDC auth works, LDAP reads succeed, PostgreSQL data persists
- Exit: dev-site acceptance signed off by Deployment Manager + HR Director (STK-001)

### Gate 2: Production Site Acceptance (with Pilot)
- Portal deployed to production Windows Server (same node profile)
- Pilot group: 10-20 employees across 3 offices for 1 week
- Exit criteria:
  - AC-001: Employee clocks in/out without help
  - AC-003: Employee finds colleague's phone/email in under 10 seconds
  - AC-004: 80% of pilot group completes at least one clocking with no prior training
  - NFR-001: Pages load under 3 seconds
  - NFR-002: Clocking responds under 1 second

### Full Rollout
- All 3 offices, all 200 employees
- Monitor adoption per BG-003 (80% within 3 months)
- R002 mitigation: communicate change to prevent Excel relapse

### Rollback Criteria
- Critical UC failure (clocking, auth, directory)
- Performance below NFR thresholds
- Data integrity issues
- Rollback action: stop production instance, redirect to maintenance page, fix on dev-site, re-deploy

## 5. External Infrastructure Dependencies (R010)

| Dependency | Provider | Needed For | Blocking? |
|---|---|---|---|
| Windows Server provisioning | STK-004 (Infra) | All deployment | Yes — blocks Construction deployment |
| Keycloak client registration | STK-004 (Infra) | OIDC authentication (all UCs) | Yes — blocks auth integration |
| LDAP read access to AD (service account) | STK-004 (Infra) | Directory search (UC-004), category assignment (UC-007) | Yes — blocks Elaboration PoC for R001 |

These dependencies are owned by STK-004 and must be secured before Construction deployment activities begin.

## 6. Deployment Constraints

| ID | Constraint | Impact on Deployment |
|---|---|---|
| CON-008 | Internal Windows Server (no cloud) | Single-node physical or VM server |
| CON-009 | Internal corporate network only | No external DNS, no public endpoints |
| CON-010 | Chrome and Edge only | No cross-browser deployment testing |
| CON-014 | Backup/recovery is Infrastructure's responsibility | Coordinate with STK-004; not a portal deliverable |
| NFR-003 | Availability Mon-Fri 7:00-19:00 | Deployment windows outside business hours |

## 7. Bill of Materials (Preview)

| Component | Technology | License |
|---|---|---|
| Application | .NET 10 Razor Pages | Open-source (MIT) |
| Database | PostgreSQL | Open-source (PostgreSQL License) |
| Authentication | Keycloak (existing) | Existing infrastructure |
| Directory | Active Directory (existing) | Existing infrastructure |
| Web Server | IIS or Kestrel (TBD in Elaboration) | Included with .NET |

Detailed BOM with lock files will be produced in Construction when the codebase is established.

## 8. Risks Affecting Deployment

| Risk | Severity | Mitigation |
|---|---|---|
| R001: AD LDAP attribute inconsistency | High (P3×I3=9) | Early PoC in Elaboration; coordinate with STK-004 |
| R002: Employee adoption resistance | Medium (P3×I2=6) | Change communication during pilot; BG-003 tracking |
| R010: Infrastructure deliverables | High | PM engages STK-004 early in Elaboration |
