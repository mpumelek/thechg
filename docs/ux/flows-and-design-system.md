# UX flows and design-system brief

Status: Draft. Owners: UX lead and product owner. Validate with branch administrators, leadership, members and low-connectivity users in each rollout country.

## Primary flows to prototype

1. Branch administrator: find possible duplicate → register draft → validate fields → submit → see approval state and required correction.
2. Approver: review submitted snapshot and history → approve/reject with reason → see next journey task.
3. Member: accept invitation → establish MFA where required → view own profile, journey and giving → request controlled change.
4. Branch finance: capture manual giving → see unsynced entries → sync → resolve rejected item → post batch → issue receipt.
5. Pastoral practitioner: open assigned case → add note → delegate limited access → close/reopen with audit.

## Design principles

Mobile-first responsive layout, plain language, translation-ready strings, accessible labels/focus/error summaries, and short forms that work on low-end devices. Use consistent status chips/text plus explanation; never use color alone. Every critical action has a confirmation and visible result. For offline capture show `Saved on this device`, `Syncing`, `Accepted`, `Needs attention`; never imply server acceptance while pending. Warn before logout/clear-storage if unsynced work exists and provide a recovery/export path approved by security.

## Components and content

Document typography, spacing, navigation, form controls, tables, validation, dialogs, status messaging, empty states, loading states and permissions-denied states in a small shared component catalogue. Keep public website and staff portal visually coherent but distinguish authenticated/private surfaces. Do not display sensitive notes in toast notifications, browser titles or URL parameters. Test keyboard navigation, zoom, narrow screens, translation length and intermittent network behavior.

## Decisions

Brand assets and languages are TBD. Household/guardian portal access is TBD. Choose a WCAG target in [NFR](../architecture/non-functional-requirements.md) before sign-off; record test evidence in UAT.
