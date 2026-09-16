# Production release-readiness checklist

Status: Draft. Owners: Release manager, security lead, product owner and operations. Record evidence links, approver and date for each line; unchecked items block launch unless an explicit risk exception is approved.

## Business and data

- [ ] Country/branch rollout scope and support contacts confirmed.
- [ ] Membership policy, approval roles, curriculum and portal visibility approved.
- [ ] Migration dry run complete; member counts and finance totals reconciled.
- [ ] Country legal/privacy and payment/receipt treatment signed off.
- [ ] Real-user UAT completed with critical defects closed.

## Security and quality

- [ ] Existing exposed credentials rotated and removed from deployed configuration.
- [ ] Default administrator password/account bootstrap removed or secured.
- [ ] Permission matrix implemented and denial tests pass.
- [ ] MFA, account recovery, session revocation and export controls tested.
- [ ] Pastoral/minor data controls and document access tested where in scope.
- [ ] Upload, webhook, offline replay and rate-limit tests pass.
- [ ] Independent security assessment has no unaccepted critical/high findings.
- [ ] Accessibility and supported-browser/mobile tests pass.

## Operations

- [ ] Production/staging configuration and secrets reviewed.
- [ ] Database migration and rollback rehearsed; backup taken.
- [ ] Backup restore drill meets approved RTO/RPO.
- [ ] Web, SQL, worker, outbox, payment, messaging and sync alerts tested.
- [ ] Incident, support and disaster-recovery runbooks rehearsed.
- [ ] Load tests meet approved capacity/performance targets.
- [ ] Release artifact, change record, owners and go/no-go meeting recorded.
- [ ] Post-launch monitoring window and rollback trigger agreed.

## Sign-off

Product owner: TBD. Church sponsor: TBD. Finance: TBD. Security/information officer: TBD. Operations: TBD. Release version/date: TBD. Evidence location: TBD.
