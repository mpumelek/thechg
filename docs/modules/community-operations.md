# Attendance, events, ministries and volunteers

Status: Draft. Owners: Operations, event and ministry leads. Build these as separate domain modules sharing stable contracts, not one table for every concept.

## Attendance engine

Own `AttendanceSession` and `AttendanceRecord`, with `(ContextType, ContextId)` pointing to a service, event session, iMihlangano meeting, curriculum class or training session. It accepts manual member search, short-lived signed QR check-in and separate anonymous counts. Identified check-in is unique per member/session. Offline check-ins carry `DeviceId` and `ClientOperationId` for deduplication. Absence thresholds create follow-up tasks only when the module's privacy policy allows them; no automatic public engagement score.

## Events

Own event/session/venue/capacity/registration/ticket/waitlist states. Registration is independent of attendance: `Registered`, `Waitlisted`, `Cancelled`, `Attended`, `NoShow` are related outcomes, not interchangeable records. Paid events request a payment attempt via the Payments contract, but sales are not charitable giving. Refunds follow recorded policy and are auditable.

## Ministries

Church-wide `MinistryDefinition` and local `BranchMinistry` have effective-dated leaders and memberships. Ministries may define skills and training prerequisites. Membership in a ministry does not grant broad system access; a leader receives only scoped permissions needed for that ministry.

## Volunteer rosters

Model `VolunteerOpportunity → Shift → RosterAssignment`. A shift holds required roles, slots, dates, venue and prerequisites. Assignment states: `Invited`, `Accepted`, `Declined`, `Cancelled`, `Completed`, `NoShow`. Substitution links old/new assignments. Enforce no overlapping accepted shifts where configured. Reminders are outbox jobs; do not send within roster transactions.

## Acceptance

Test duplicate/forged QR, offline replay, event capacity race, waitlist promotion, ministry-scoped access, roster conflicts and substitution history.
