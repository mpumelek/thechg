# Membership module specification

Status: Draft. Owners: Membership lead and product owner. Policy approval: Church leadership. Related: [workflow rules](../product/business-rules-and-workflows.md), [permissions](../architecture/permissions-matrix.md).

## Boundary and model

Owns `Member`, `MembershipApplication`, profile/contact records, family/guardian relationships, branch assignment history, status history, duplicate-review cases and member change requests. Identity owns login credentials; journey owns curriculum progress; finance owns giving. The member aggregate enforces primary branch, membership state and transitions, not every related activity.

## States and commands

`Draft → PendingApproval → Approved → PreActive → Active`; `PendingApproval → Rejected → Draft` for revision; `Active → Suspended/Inactive → Active`; eligible records may be `Archived` under retention policy. `Approved` means accepted into joining process, not necessarily active. Commands: `RegisterMember`, `UpdateDraft`, `SubmitApplication`, `ApproveApplication`, `RejectApplication`, `ActivateMember`, `SuspendMember`, `ReactivateMember`, `RequestTransfer`, `CompleteTransfer`, `RequestProfileChange`.

Required invariants: authorized branch administrator creates record; one open application per member; approver holds `Member.Approve` in scope; invalid transition fails; activation requires journey completion or approved exception; branch transfer preserves prior effective-dated assignment and does not rewrite historical transactions. Duplicate detection flags likely matches for review rather than silently merging persons.

## Member portal

An approved person may establish an account privately at a branch after identity verification; no invitation is used. Link `ApplicationUser` and `Member` explicitly. A pending account request is not official membership. Read own profile by server-side link, not by caller-selected ID. Controlled fields such as legal name, date of birth, branch, status and guardian link use change requests. Household visibility is not granted automatically; see `DEC-005`.

## Events, audit and acceptance

Emit `MemberRegistered`, `MembershipSubmitted`, `MemberApproved`, `MemberRejected`, `MemberActivated`, `MemberTransferred`, `MemberSuspended`. Audit submitted snapshots, decisions, actor, scope, reason and timestamp. Test anonymous registration denial; cross-branch ID substitution denial; duplicate handling; concurrent approvals; transfer history; invalid state transitions; member viewing another member's record.
