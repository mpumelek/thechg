# ADR 0004: Initial hosting with an independent worker

Status: Proposed. Date: 2026-09-16. Owner: Operations lead. Approver: Sponsor, information officer and technical lead — TBD.

## Context

SmarterASP is the preferred candidate for initial web and SQL hosting. The platform also requires timely payment processing, retries, notifications, exports and offline sync. SmarterASP documents a Premium scheduled task as an HTTP GET caller with a normal minimum 15-minute interval; that is not a continuous job processor. [Provider documentation](https://www.smarterasp.net/support/kb/a2385/how-to-schedule-a-task.aspx)

## Decision

Keep the app cloud-neutral. If SmarterASP passes security, data-location, backup and capacity review, host web/API and SQL there initially. Run a separate always-on .NET worker on a suitable managed host, processing a SQL transactional outbox. Use private external object storage and independent monitoring/backups. Reassess a unified managed cloud platform before production commitment.

## Alternatives considered

Do all work inside IIS web app: vulnerable to app-pool recycling. Use Premium HTTP scheduler as sole worker: unsuitable for time-sensitive/payment work. Replatform everything immediately to Azure: architecturally clean but may exceed current budget/readiness.

## Consequences

Reliable asynchronous processing but two hosting environments to operate. SQL connectivity, credential management, latency and jurisdiction must be verified in a proof of concept. This ADR is conditional, not approval of a particular hosting plan or data region.
