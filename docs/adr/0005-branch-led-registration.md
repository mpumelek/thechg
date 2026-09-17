# ADR 0005: Branch-led registration without invitations

Status: Product direction confirmed by user on 17 September 2026; security and approval-policy details proposed, not Church-approved. Owners: Product owner and security architect. Approvers for staff authority, membership approval and bootstrap: Church leadership — TBD.

## Context

The previous draft assumed invitation-based account setup. The user clarified that **all registration, including staff and members, happens at branches** and that nobody will be invited. Account creation, official membership and staff authority remain separate concerns.

## Decision and current implementation boundary

Only an authenticated, actively scoped branch registrar may capture an account request. Member requests require `Member.Create`; staff requests require the distinct `Staff.Register` permission. The server loads the branch and derives its `ChurchId`; the requester cannot choose authority by supplying a Church ID. Capture records the registrar, branch, account and kind, and creates an inactive account without a password, confirmed email, membership record or permission grant. Duplicate email is rejected for review rather than merged automatically. Both kinds remain `PendingReview`. There is no public self-registration or invitation issuance/redemption.

Credential setup must occur privately in person at the branch after identity verification and applicable approval. A registrar must never know or assign the person's password. Staff-role approval, account activation, MFA, member-account linkage and first-registrar bootstrap are separate, audited workflows and are **not** authorized or implemented by this ADR. No real staff or member onboarding is enabled merely by this pending capture route.

## Consequences and open decisions

This removes email/SMS invitations as a registration dependency, but branches need a controlled device/process for private credential setup and duplicate/identity checks. Church leadership must decide who approves staff access, which roles may register staff, whether separation of duties is mandatory, how the first registrar is bootstrapped, and how members without email credentials are handled. Until then, pending accounts cannot sign in and no administrative grant/activation endpoint is exposed. A new additive migration retires the old invitation table only if it is empty, preventing silent data loss.
