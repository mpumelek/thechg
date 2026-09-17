# Requirements and prioritized backlog

Status: Draft. Owner: Product owner. Approval: Relevant church leadership per domain. Each item needs a linked implementation issue and UAT evidence before closure.

## Requirements convention

Use `REQ-<domain>-<number>` as a stable ID. A story is ready only when actor, scope, preconditions, success path, failure path, data classification, permissions, and measurable acceptance criteria are known. The [business rules](business-rules-and-workflows.md) take precedence over UI sketches; the [architecture blueprint](../architecture/church-management-platform-blueprint.md) governs technical boundaries.

## Foundation and membership — highest priority

| ID | Requirement | Acceptance evidence |
|---|---|---|
| REQ-ORG-001 | Manage country → circuit → branch hierarchy with effective leadership assignments | New branch can be created without schema change; sibling branch records remain inaccessible |
| REQ-IAM-001 | Capture all staff and member registrations in person at authorized branches; assign staff permissions only after separate approval | No invitations or public self-registration; capture remains pending; revoked assignment removes access; privileged MFA enforced |
| REQ-MEM-001 | Only an authorized branch administrator registers an official member | Anonymous/member self-registration cannot create a member; audit records actor and branch |
| REQ-MEM-002 | Submit, approve, reject, resubmit, suspend, reactivate and archive with explicit state rules | Invalid transitions return conflict; every decision records actor, reason and timestamp |
| REQ-MEM-003 | Prevent member becoming active before required approval and journey completion or recorded exception | Domain and API tests cover both paths |
| REQ-MEM-004 | Transfer member between branches without rewriting history | Prior branch remains on historic giving/attendance; transfer approval policy applied |
| REQ-PORTAL-001 | Approved member sees only own permitted profile and giving records | Cross-member identifier substitution is denied; sensitive fields excluded |
| REQ-PORTAL-002 | Controlled profile changes use a request workflow | Member cannot directly change protected fields; decision is auditable |

## Journey and community

| ID | Requirement | Acceptance evidence |
|---|---|---|
| REQ-JRN-001 | Enrol against an immutable curriculum version | Later curriculum edits do not alter existing enrolment criteria |
| REQ-BUD-001 | Assign eligible buddy within branch/capacity policy | Over-capacity and out-of-scope assignments are blocked; reassignment retains history |
| REQ-GRP-001 | Configure iMihlangano schedules and dated member allocation | Tuesday/Thursday are defaults, not fixed fields; transfers retain history |
| REQ-ATT-001 | Capture identified and anonymous attendance for multiple contexts | Duplicate member check-ins prevented; QR token is short-lived |
| REQ-EVT-001 | Register for events independent of attendance | Capacity/waitlist and paid/free policy tested |

## Financial, communications and operations

| ID | Requirement | Acceptance evidence |
|---|---|---|
| REQ-FIN-001 | Capture manual giving, including offline queue | Replayed client operation creates one contribution; pending work is visible |
| REQ-FIN-002 | Record online giving only after verified provider confirmation | Forged/duplicate webhooks cannot post twice; reconciliation report works |
| REQ-FIN-003 | Correct posted contributions by reversal/adjustment | Original is immutable; audit and receipt treatment visible |
| REQ-COM-001 | Send templated email/SMS/WhatsApp via replaceable adapters and consent rules | Provider failure retries and dead-letters without rolling back business transaction |
| REQ-SEC-001 | Restrict pastoral, minor, finance and document data beyond branch access | Denial and sensitive-read audit tests pass |
| REQ-OPS-001 | Back up and restore production data | Restore rehearsal meets approved RTO/RPO |

## Later backlog

Ministries/volunteer rostering, store/inventory, pastoral and prayer workflows, executive reporting, engagement framework, accounting export, and native mobile app. Create module-specific acceptance criteria before scheduling each, rather than treating this list as an implementation specification.

## Definition of done

Code, migrations, tests, API contract, permission matrix, audit events, monitoring, support instructions, accessibility review and UAT evidence are updated. No story is complete solely because its happy-path screen works.
