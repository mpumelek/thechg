# Business rules and workflow catalogue

Status: Draft. Owner: Product owner. Policy approval: Church leadership; finance leadership for financial rules. Open policy questions are recorded in [decision-register.md](decision-register.md).

## Member registration and approval

Actor: scoped branch administrator. Preconditions: assigned branch, required identity/contact data, duplicate search. Register as `Draft`; the member is not an active official member and cannot grant themself member access. Submit changes state to `PendingApproval`, freezes the submitted snapshot and creates a leadership task. An approver with `Member.Approve` for that scope approves or rejects with a reason. Rejected applications may be revised and resubmitted; the prior decision remains immutable. Approved applications start `PreActive` membership and the new-member journey. `Active` requires published curriculum completion or authorized exception. Suspension, reactivation and archiving are separate recorded transitions.

Policy TBD: exactly which leadership roles may approve, quorum/second decision if any, duplicate resolution, minimum fields, exception authority, and appeal procedure. See [membership specification](../modules/membership.md).

## Branch transfer

A permitted initiator creates a transfer request with source, target, effective date and reason. The target branch must exist and be active. On approval, close the old branch assignment and open the new one atomically; do not update historic contributions, attendance, cases or audit events. Pending buddies, iMihlangano allocation and volunteer shifts are separately reviewed. Policy TBD: whether source and/or destination approval is required and whether members may request transfers in the portal.

## New member journey

An approved application enrols against a published `CurriculumVersion`. A coordinator assigns a buddy and tracks orientation, lessons, attendance, assessments and church-defined milestones. Completion is calculated against that version, with recorded evidence and exception decisions. Graduation activates membership only when all other prerequisites hold. Curriculum content and baptism/confirmation applicability require Church leadership approval. See [journey specification](../modules/new-member-journey.md).

## Giving and correction

Manual giving is captured into a branch batch. Offline entries are pending until accepted by the server. The server validates member/branch/fund/currency, duplicate operation ID and period; posting makes transactions immutable. Online giving starts with a payment attempt and posts only after verified provider webhook plus reconciliation checks. Corrections create linked reversal/adjustment entries. Refunds follow a separate authorization policy; routine posting has no mandatory dual approval. Policy TBD: receipt wording, anonymous-giving rules, period closing, refund authority and country tax/accounting treatment.

## Events, store and pastoral exceptions

Event registration is not attendance. Store purchases are not donations. Pastoral escalation creates a restricted case and never copies confidential narrative into general tasks or messages. Prayer visibility (`Public`, `Team`, `Confidential`, `Anonymous`) is chosen and enforced per request; public testimony requires separate consent. These rules must be revisited during each module's UAT.
