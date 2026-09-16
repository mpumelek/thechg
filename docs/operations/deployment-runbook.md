# Deployment and environment runbook

Status: Draft. Owner: Operations lead. Replace placeholders with provider-specific commands only after hosting ADR, access and environment design are approved. Never commit production credentials.

## Environments

Local development uses synthetic data and isolated secrets. Test runs automated integration/API checks. UAT/staging mirrors material production dependencies with sanitized data. Production has restricted access, separate credentials and independently stored backups. Confirm actual web, SQL, worker and object-storage regions before provisioning.

## Release procedure

1. Identify signed/versioned artifact and approved change record.
2. Check previous backup and current restore test; take pre-release backup if schema/data change.
3. Validate staging deployment, contract tests, migration script and rollback path.
4. Announce change window; pause or drain workers if migration requires it.
5. Apply reviewed expand-compatible database migration once, with log and operator.
6. Deploy web/API and worker artifact from the same release version.
7. Verify health, login, permissions, member read, outbox processing, webhook endpoint and file access.
8. Resume workers/traffic; monitor error, latency and reconciliation dashboards.
9. Record completion and any follow-up issue.

## Rollback

Prefer application rollback to the previous compatible artifact. Destructive schema rollback is exceptional; use expand/contract migration design and restore only under approved disaster procedure. If new financial transactions occurred, finance must reconcile before restoring older data. Do not rerun a non-idempotent migration blindly.

## Hosting note

SmarterASP may host web and SQL if country/legal, capacity and backup review passes; keep an independent always-on worker. Its documented scheduled task is an HTTP GET caller with normal 15-minute minimum on Premium, not a substitute for the worker. [Provider documentation](https://www.smarterasp.net/support/kb/a2385/how-to-schedule-a-task.aspx)
