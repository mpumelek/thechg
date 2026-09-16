# Legacy data migration and cutover plan

Status: Draft and **optional future data-import workstream**. The new application is being built from scratch; no legacy source code or data has been imported. Owners: Database architect, product owner, finance lead and operations. Do not run against production before backups, mapping approval and rehearsal.

## Scope and source inventory

The current repository has `Circuit`, `Branch`, `Member`, `TitheRecord`, `MonthlyOffering`, news/gallery/settings and ASP.NET Identity tables. Capture source schema version, row counts, null/duplicate patterns, image paths, role assignments, and referential integrity. Keep a read-only source snapshot and assign a migration batch ID. Existing credentials must be rotated first.

## Mapping principles

Map old circuits/branches to `OrganizationalUnit` with stable crosswalk IDs. Map members to new `Member` plus status/history only after Church leadership decides whether existing records are automatically approved/active. Link existing Identity users to members only when identity can be verified; never infer from similar names alone. Map tithes/monthly offerings to finance records with finance approval of meaning, period, branch/currency and posted state. Published media is scanned/classified before private/public storage placement.

## Rehearsal and reconciliation

1. Back up and restore source to isolated rehearsal environment.
2. Run deterministic extract/transform/load with versioned scripts and per-row error log.
3. Reconcile source/target counts, sums by branch/period/currency, identity links and sample documents.
4. Validate user access and denied-access cases in target.
5. Run UAT with at least one branch per country when known.
6. Measure migration time and rollback time; repeat until within approved outage window.

## Cutover

Announce freeze; take final backup; confirm recovery point; stop writes on old system; run migration; reconcile; switch traffic; run smoke/security/payment checks; monitor errors and outbox. Rollback criteria include failed reconciliation, unexpected data exposure, inability to authenticate or failed critical transactions. Rollback restores old traffic and data state with finance-led reconciliation for any target transactions created after cutover. Never delete the source until retention and sign-off are complete.
