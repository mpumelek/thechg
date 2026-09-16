# Pastoral care and prayer module specification

Status: Draft. Owners: Pastoral leadership, safeguarding lead and security architect. No pastoral feature should be enabled before case classification and emergency-access policy are approved.

## Pastoral boundary

`PastoralCase`, participants, restricted access grants, confidential notes, care tasks and referrals live in a separate module/schema. Case access needs explicit practitioner/team grant and purpose; ordinary branch administration and even broad executive roles have no default access. Read access is logged. Break-glass access requires designated actor, reason, expiry and alert. Avoid copying case narrative into email, generic notifications, search indexes, audit diffs or analytics.

Case states: `Open`, `OnHold`, `Referred`, `Closed`, `Reopened`. Notes may be addenda/corrections, not silently overwritten. High-risk safeguarding or crisis cases follow an approved human escalation policy, not an automated diagnosis. Retention and legal basis must be reviewed per country.

## Prayer requests

`PrayerRequest` has requester optional, visibility (`Public`, `PrayerTeam`, `Confidential`, `Anonymous`), consent record, assigned team, status and outcome. Anonymous means the requester is not shown to recipients; if the system retains identifying metadata, that must be disclosed and protected. Public display and later testimony each need explicit publication approval/consent. Escalating to pastoral care creates a case link with minimum necessary detail and separate access checks.

## Acceptance

Branch administrator denied confidential case; named practitioner allowed and read audited; revoked grant immediately denies; break-glass alert emitted; public prayer list excludes confidential content; child case requires safeguarding-specific access.
