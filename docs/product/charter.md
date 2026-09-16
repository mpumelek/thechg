# Product charter

Status: Draft. Owners: Church sponsor and product owner. Approval: Church executive leadership. Review before Phase 1.

## Purpose and outcomes

Create one secure digital operating platform for the denomination across four stated countries, about 25 circuits and 175 branches. It must support church administration and member self-service without weakening local branch governance. The first outcome is a member being registered by an authorized branch administrator, approved by permitted leadership, invited to an account, and able to see only their own information.

## Users

Church executive and senior leadership; national/country, circuit and branch administrators; Amalunga; Abafundisi/Abafundikazi; pastors; membership and finance users; ministry, iMihlangano and volunteer leaders; pastoral-care users; buddies; approved members and guardians. Public visitors can see published website content, not internal records.

## Scope and sequence

- Phase 0: establish the new solution, secure configuration, tests, CI, and an approved data-import strategy if legacy data is later brought across.
- Phase 1: organizational structure, identity, scoped authorization, member registration/approval, audit.
- Phase 2: member portal, change requests, secure documents, initial giving history.
- Phase 3: journey, buddy, iMihlangano, attendance.
- Phase 4: giving, online payments, receipts, offline manual capture.
- Later: ministries, events, pastoral/prayer, communications expansion, store, analytics.

The previous repository's application code is not an implementation input. Any later legacy data import is a separate, approved workstream. A native mobile application, true SaaS support for unrelated churches, separate country databases, advanced warehouse analytics, and a universal workflow engine are outside the initial release.

## Success measures and acceptance

Measure: percentage of branches onboarded; approval turnaround; member-portal adoption; member-record duplicate rate; successful offline synchronization; payment reconciliation exceptions; support incidents; backup restore success; authorization-test pass rate. Numeric targets are **TBD** after volume and leadership review. A release is accepted only when its documented workflows pass UAT, security controls pass, and operations can recover it.

## Governance

The product owner prioritizes scope; church leadership owns theological and membership policy; the information officer owns privacy decisions; finance leadership owns giving and refund rules; the technical lead owns implementation design within approved ADRs. Changes to policy, legal basis, data sharing, or financial treatment require named approval and an updated acceptance test.

## Constraints and assumptions

One denomination; four stated long-term countries (South Africa, Eswatini/Swaziland, Mozambique and Zimbabwe), with **South Africa as the only current build/pilot scope and Mozambique next**; Eswatini/Swaziland and Zimbabwe are unscheduled. Web first; later mobile; manual and online giving; poor connectivity at some branches; no routine two-person approval for ordinary finance posting. Remaining rollout timing, member volumes by country, budget, target availability, household visibility, hosting location and payment providers remain open in the [decision register](decision-register.md).
