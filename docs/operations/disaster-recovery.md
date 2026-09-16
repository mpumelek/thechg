# Backup and disaster-recovery plan

Status: Draft. Owners: Operations lead and Church sponsor. RTO/RPO and maximum tolerable outage are **TBD**; approve them before choosing backup frequency or production host. See [Microsoft DR guidance](https://learn.microsoft.com/en-us/azure/well-architected/design-guides/disaster-recovery).

## Protected assets

Operational SQL database; object storage; application artifacts and configuration; encryption/data-protection keys; provider configuration and webhook secrets; outbox/job state; audit/log evidence. Backups must be encrypted, access-controlled and copied outside the primary hosting account. Backup locations and retention require country privacy review.

## Recovery procedure

1. Incident commander declares disaster and selects known-good recovery point.
2. Freeze writes and payment processing or route to a safe holding state.
3. Verify backup integrity, restore SQL to isolated target, restore object metadata/files and key material.
4. Deploy compatible web/API and worker artifacts with reviewed secrets.
5. Validate schema, row counts, member login/scope, document access, outbox state and financial reconciliation.
6. Reconcile provider webhooks/settlements received during outage idempotently.
7. Cut traffic over, communicate status, monitor and document actual RTO/RPO.
8. Plan failback only after data divergence is understood and approved.

## Testing and ownership

Run a timed restore rehearsal at least before first launch and at a cadence approved by operations. Record backup ID, steps, elapsed time, data loss, failures and corrective actions. A backup-job success alone is not recovery evidence. Named roles: incident commander TBD; database restore lead TBD; application/worker lead TBD; finance reconciliation lead TBD; communications/legal lead TBD.
