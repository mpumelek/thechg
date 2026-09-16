# Non-functional requirements and service targets

Status: Draft. Owners: Product owner, solution architect and operations lead. Numeric values below are **proposed planning targets**, not an approved SLA. Approve after member volumes, locations, budget and hosting are confirmed.

| ID | Requirement | Proposed verification / open target |
|---|---|---|
| NFR-01 Availability | Define monthly uptime separately for public site and core administration; `TBD` target and exclusions |
| NFR-02 Recovery | Leadership approves maximum tolerable outage, RTO and RPO before production; restore drill measures actual values |
| NFR-03 Performance | Define p95 response target for normal page/API reads and capture; test with realistic branch/country data, `TBD` ms |
| NFR-04 Capacity | Size for `TBD` members, concurrent users, Sunday check-ins, payments/hour and storage growth |
| NFR-05 Offline | Pending operations survive app restart; visible sync status; `TBD` maximum offline duration and device policy |
| NFR-06 Correctness | Duplicate payment webhooks/offline operations produce one business transaction; finalized finance history is immutable |
| NFR-07 Security | Privileged MFA, least privilege, encrypted transport/storage, audited exports and sensitive reads |
| NFR-08 Privacy | Purpose/retention/classification defined for every personal data category and country |
| NFR-09 Accessibility | Responsive keyboard and assistive-technology usable flows; agree target WCAG version/level before UAT |
| NFR-10 Observability | Correlated logs, request/error metrics, outbox age, failed jobs, payment lag, sync conflicts and backup alerts |
| NFR-11 Operability | Staging mirrors material production dependencies; documented release/rollback and support ownership |
| NFR-12 Maintainability | Module boundary tests, automated migration tests, OpenAPI contract checks and repeatable local setup |

## Capacity scenarios to test

1. Sunday morning: many branches checking in concurrently, leadership viewing dashboards.
2. Connectivity returns: multiple branches replay queued member/attendance/giving operations.
3. Provider retries: duplicate and out-of-order payment webhooks.
4. National export: large report without blocking ordinary users.
5. Site/worker/database outage: backup restoration and recovery communication.

Do not turn proposed targets into contractual commitments without load tests and provider confirmation. RTO/RPO should come from business impact and drive backup/deployment choices, as described in [Microsoft's business-needs guidance](https://learn.microsoft.com/en-us/azure/architecture/guide/design-principles/build-for-business).
