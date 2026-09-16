# Test strategy and traceability

Status: Draft. Owners: QA lead and technical lead. Requirements link to test IDs and UAT evidence; coverage is risk-based, not only a code percentage.

| Level | Focus | Minimum evidence |
|---|---|---|
| Domain unit | Member states, journey requirements, buddy capacity, posted-finance immutability | Deterministic pass/fail cases and edge conditions |
| Application | Permissions, scope, commands, idempotency, outbox | Positive and denial cases per use case |
| Database integration | EF mappings, constraints, migration, concurrency | Real SQL Server test database or equivalent container |
| API contract | OpenAPI, validation, status/problem responses, pagination | Contract diff and request/response tests |
| UI/end-to-end | Registration/approval/portal/giving/offline | Browser flows on desktop/mobile widths |
| Security | IDOR, session, CSRF, upload, webhook, minors/pastoral access | Threat-model-linked tests and independent review |
| Performance | Sunday check-ins, sync burst, payment burst, reports | Load profile and p95/error results vs approved NFR |
| Recovery | Backup restore, worker restart, provider outage | Timed drill and reconciliation evidence |
| UAT | Real branch/country process | Named business sign-off and open-defect list |

## Highest-risk test matrix

For each permission, test same branch, sibling branch, parent circuit/country, revoked assignment, self member, unrelated member and sensitive record. For payment/offline commands, test duplicate, out-of-order, interrupted, replayed and concurrent submissions. For migration, compare counts and financial totals by source ID, branch, period and currency. For minors/pastoral data, assert absence from unauthorized response, export, log and search results.

## Definition of done

Every requirement has test links; failing critical/high security or financial tests block release. Record known exceptions with risk owner and expiry. UAT does not substitute for automated regression. Test data must be synthetic or approved de-identified data.
