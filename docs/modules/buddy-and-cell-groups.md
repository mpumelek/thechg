# Buddy and iMihlangano specifications

Status: Draft. Owners: New Member Coordinator and iMihlangano ministry lead. Related: [journey](new-member-journey.md), [attendance](community-operations.md).

## Buddy management

Own `BuddyProfile` (eligibility, training, availability, preferred language, branch restrictions, capacity), `BuddyAssignment` (journey, dates, status, reason), check-ins, follow-up tasks and escalation metadata. A member may have multiple historical buddies but at most one current primary assignment for a journey. Assignment checks eligibility, consent, branch policy and workload within one transaction. Reassignment closes the old assignment and opens a new one without erasing history. A buddy sees only journey support details permitted by policy; no automatic pastoral access.

Workflow: candidate search → capacity/branch validation → coordinator assignment → member/buddy notification → scheduled check-ins → completion or reassignment. Missed tasks escalate to a coordinator, not automatically to a pastoral case. Reporting covers workload, overdue follow-ups and completion, with context and no simplistic person ranking.

## iMihlangano

Own `CellGroup`, schedule/exception, venue, language/life-stage/capacity configuration, leader assignments, member allocations, meeting records and announcements. Tuesday/Thursday schedules are seeded options, not fixed columns. Group allocation is effective-dated. Transfers close the old allocation and open a new one. Group search may filter branch, distance (only if location permission exists), day, language, life stage and capacity.

Group attendance uses shared `AttendanceSession` and `AttendanceRecord`, linked by context ID. Group health reporting uses trends and follow-up backlog; it does not expose confidential notes. Private home locations need explicit visibility settings.

## Acceptance

Over-capacity buddy assignment fails; reassignment retains history; group schedules can change without migration; group transfer retains prior allocation; an ordinary member cannot see a private meeting address or another member's buddy notes.
