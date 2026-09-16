# Support and routine operations runbook

Status: Draft. Owner: Operations/support lead. Fill actual contacts, URLs, metrics and escalation rota before production. Never paste member/pastoral/payment details into general tickets.

## Triage

Collect timestamp, environment, correlation ID, affected branch/country, operation type and business impact. Classify: `P1` security breach, widespread outage or financial integrity failure; `P2` major branch/service disruption; `P3` limited functional issue; `P4` question/enhancement. Exact response targets are TBD in the [NFR](../architecture/non-functional-requirements.md).

## Common procedures

| Symptom | First checks | Safe action / escalation |
|---|---|---|
| Member cannot sign in | Account status, invitation expiry, MFA/session status | Use approved reset/invite; never share temporary password |
| Approval stuck | Application state, assignment scope, outbox task age | Reassign through audited workflow; do not edit DB directly |
| Offline queue stuck | Device status, network, client operation IDs, rejection reasons | Preserve/export pending queue; retry idempotently |
| Missing receipt | Contribution posted? outbox/delivery attempt? | Retry message only; do not duplicate contribution |
| Payment mismatch | Attempt, verified provider event, settlement report | Finance reconciliation case; never manually assert success from browser screenshot |
| Worker backlog | Lease age, error/dead-letter count, provider status | Restart/scale worker only after preserving failed items |
| Unauthorized data report | Resource and access logs | Treat as security incident; restrict access and preserve evidence |

Review backups, capacity, failed jobs, payment reconciliation and role grants on a defined schedule. Document every manual correction and approver. Link to [incident response](incident-response.md) and [disaster recovery](disaster-recovery.md).
