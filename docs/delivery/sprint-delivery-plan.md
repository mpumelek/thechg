# Proposed epic backlog and sprint delivery sequence

Status: **Transferred draft backlog requiring re-baselining for the new repository**, 16 September 2026. Owner: Product owner; technical lead maintains dependencies; Church, finance, security and information-officer approvals remain with their named owners. This is a forecast, **not** an approved date, budget, SLA, policy or release commitment. The new .NET 10 solution scaffold exists, but product features are not implemented.

The original S1–S2 security, regression and migration stories describe work on the previous repository. They are historical, not instructions to import its code. Re-scope those stories around the clean solution and treat any legacy **data** import as a separately authorized workstream before assigning new sprint dates. The epic order and product outcomes remain planning inputs.

**Scope update, 16 September 2026:** the user identified **four long-term countries**—South Africa, Eswatini (Swaziland), Mozambique and Zimbabwe—then clarified **build and pilot for South Africa only now; Mozambique next**. Eswatini/Swaziland and Zimbabwe are unscheduled. The former S24–S25 later-country build/pilot slots are withdrawn pending re-baseline after South Africa evidence; do not interpret S40 as a four-country completion date.

## Planning basis and priority

- Assume two-week sprints, one cross-functional squad (roughly four engineers, QA, shared UX/operations/security, and an available product owner), and a usable staging environment. Re-baseline after S2 using actual capacity and again at every release gate. If staffing differs, keep the dependency order and move sprint numbers; do not compress security, finance or country approvals.
- Reserve capacity every sprint for tests, accessibility, documentation, defects, security review, migration compatibility and a demonstrated vertical slice. Stories are not pre-estimated points; split any item that cannot be completed within one sprint during refinement.
- **P0 — release blockers:** exposed-secret remediation, test/CI baseline, organizational scope, Identity, membership workflow, migration, security and recovery. **P1 — core value:** member portal, journey/activation, manual and online giving, communications, offline capture and private documents. **P2 — expansion:** buddy/groups, attendance, events, ministries, pastoral/prayer/children and advanced reporting. **P3 — optional later releases:** store and native mobile app.
- This sequence brings giving forward relative to the draft architecture's broad phase table because member-visible giving, online/manual payments and offline branch capture are explicit user needs. It does **not** mark any proposed ADR, Church policy or provider choice as approved.
- S1–S23 are a candidate South Africa web/core **pilot** sequence; S24–S40 are directional expansion slots, not scheduled work for the other countries. S23 does not mean all South African or ~175 long-term branches are live: onboarding proceeds in measured waves. Mozambique gets a separate later pilot only after its legal, hosting, provider and UAT gates; Eswatini/Swaziland and Zimbabwe remain unscheduled. No real database, payment, message or production environment is touched merely by approving this plan.

## Epics, ranked by dependency and value

| Rank | Epic | Priority | Outcome and key dependencies |
|---:|---|---|---|
| 1 | E01 Security and governance | P0 | Remove exposed credentials; agree country/privacy and policy gates before live data. |
| 2 | E02 Engineering and operations foundation | P0 | CI, tests, modular boundaries, staging, worker/outbox, observability and recovery. |
| 3 | E03 Church organization | P0 | One Church with country → circuit → branch hierarchy and effective-dated scope. |
| 4 | E04 Identity and authorization | P0 | Separate accounts from members; scoped grants, MFA, revocation and denial tests. Depends on E03. |
| 5 | E05 Membership | P0 | Branch registration, decisions, history, transfer and activation guard. Depends on E04. |
| 6 | E06 Member portal | P1 | Verified account link, own profile/giving, change requests and documents. Depends on E04–E05. |
| 7 | E07 New-member journey | P1 | Versioned curriculum and evidence-based activation. Depends on E05 and Church policy. |
| 8 | E08 Manual giving | P1 | Immutable, scoped and currency-aware contribution ledger. Depends on E03–E04. |
| 9 | E09 Online payments | P1 | Verified provider events, reconciliation and country adapters. Depends on E08, E02 outbox and finance decisions. |
| 10 | E10 Communications | P1 | Consent-aware email, SMS and WhatsApp through outbox/provider adapters. Invitations and receipts depend on it. |
| 11 | E11 Offline branch capture | P1 | Device-scoped queue, idempotent sync and visible recovery. Depends on E08 and device policy. |
| 12 | E12 Legacy migration and country rollout | P0 | Rehearsed mapping, finance reconciliation, UAT and staged country cutovers. Depends on all applicable release gates. |
| 13 | E13 Private documents and approvals | P1/P2 | Authorized storage and downloads first; generalized approval coordination later. |
| 14 | E14 Community operations | P2 | Buddy, iMihlangano, attendance, events, ministries and volunteers. |
| 15 | E15 Pastoral, prayer and children | P2 | Restricted cases and safeguarding; blocked by DEC-013/014 and privacy review. |
| 16 | E16 Reporting and engagement | P2 | Scoped operational/executive reports; versioned, explainable engagement only after policy review. |
| 17 | E17 Store | P3 | Separate commercial catalogue, inventory, orders and refunds; never charitable giving. |
| 18 | E18 Native mobile | P3 | Reuse versioned APIs and sync; start only after stable web flows and mobile business case. |

## Story register

Each story ID is stable for issue tracking. The sprint is a forecast slot, not an estimate. Acceptance evidence below is the minimum specific proof in addition to the [test strategy](test-strategy.md) and the common definition of done below.

### E01–E05 — safe organizational and membership foundation

| Story | Sprint | User outcome / minimum acceptance evidence |
|---|---:|---|
| E01-01 | S1 | Product owner records countries, rollout order, volumes, decision owners and unresolved `DEC-*`; no TBD is silently treated as policy. |
| E01-02 | S1 | Authorized owner rotates committed database/default-admin credentials, removes bootstrap secrets from deployable code and proves isolated secret injection; no live action without explicit operational authorization. |
| E01-03 | S2 | Inventory all existing routes and apply default-deny/anti-forgery and scope checks; anonymous and sibling-branch denial tests pass. |
| E01-04 | S2 | Information officer drafts per-country data flow, residency, processor, retention and child/pastoral treatment; records external legal review still required. |
| E01-05 | S22 | Independent security review of the launch scope closes critical/high findings or records a named, time-limited risk decision; verify sensitive-read/export logs. |
| E02-01 | S1 | Repeatable isolated local/test setup and CI build, test, secret/dependency scan; baseline regression tests cover current member/branch/finance behavior. |
| E02-02 | S3 | Modular-monolith skeleton, module boundary tests, EF migration pattern and versioned API conventions exist without a wholesale rewrite. |
| E02-03 | S4 | Staging and hosting/worker proof of concept use separate secrets; backup and restore to an isolated target are rehearsed, not production provisioning by assumption. |
| E02-04 | S7 | SQL transactional outbox plus independent worker retries and dead-letter handling; business write and event enqueue are atomic. |
| E02-05 | S17 | Correlated logs, provider/outbox alerts, support and incident runbooks demonstrate a failed delivery alert and recovery path. |
| E02-06 | S22 | Approved load scenarios and timed restore/rollback rehearsal meet signed-off NFR targets; otherwise launch is blocked or formally reduced in scope. |
| E03-01 | S3 | `ChurchId` and country/circuit/branch model reject cycles, invalid parents and orphaning; build/configure for South Africa only while keeping the schema country-neutral for later expansion. |
| E03-02 | S4 | Country timezone/currency and effective-dated leadership settings work; circuit/branch descendant rules pass tests. |
| E03-03 | S4 | Legacy circuit/branch crosswalk maps source IDs to target units; unmapped/duplicate rows are reported, never silently assigned. |
| E04-01 | S5 | `ApplicationUser` is distinct from `Member`; verified, expiring invitation and account link cannot create official membership. |
| E04-02 | S5 | Permission catalogue and dated role assignments enforce Church, country, circuit, branch and descendant scope; delegation cannot widen the grantor's authority. |
| E04-03 | S6 | Resource authorization rejects guessed IDs, sibling branches, unrelated members and protected records on server reads/writes and exports. |
| E04-04 | S6 | Privileged MFA, recovery and account/assignment revocation work; revoked access fails within the agreed session window and is audited. |
| E05-01 | S7 | Member, application, status and effective-dated branch history schema has business keys and migration tests; Identity remains separate. |
| E05-02 | S7 | Authorized branch administrator creates/edits a draft; anonymous/member self-registration is denied; likely duplicates go to review rather than auto-merge. |
| E05-03 | S8 | Submit, approve, reject and resubmit preserve submitted snapshot, decision actor/reason/time; invalid or concurrent transitions fail safely. |
| E05-04 | S8 | `Approved`/`PreActive` does not mean `Active`; activation requires journey completion or a later authorized recorded exception. |
| E05-05 | S10 | Effective-dated transfer and protected-field change requests preserve historical branch on giving/attendance and retain approval history. |

### E06–E13 — member experience, giving and controlled rollout

| Story | Sprint | User outcome / minimum acceptance evidence |
|---|---:|---|
| E06-01 | S9 | Invited approved member signs in and sees only their own permitted profile resolved from account link; substituted member ID is denied. |
| E06-02 | S14 | Member sees only their own posted giving, original currency, corrections and receipt status; no household access by inference. |
| E06-03 | S10 | Member submits controlled profile change and sees status; protected fields change only after scoped decision and audit. |
| E06-04 | S20 | Member downloads only own permitted private documents; expired links and another member's document are denied. |
| E07-01 | S11 | Church-authorized editor publishes immutable `CurriculumVersion`; existing enrolments do not change when a later version is published. |
| E07-02 | S11 | Coordinator/member views progress and evidence with unmet requirements; out-of-scope edits and duplicate evidence fail. |
| E07-03 | S12 | Completion or authorized exception requests activation through Membership; approval alone and unauthorized override cannot activate. |
| E08-01 | S13 | Funds, currency and contribution schema use decimal money, transaction-time branch and immutable posted records. |
| E08-02 | S13 | Scoped finance user captures and posts a manual branch batch; duplicates, invalid fund/currency and cross-branch writes are rejected. |
| E08-03 | S14 | Posted giving correction uses linked reversal/adjustment with actor/reason; original row remains unchanged. |
| E08-04 | S14 | Receipt and basic branch reconciliation report distinguish posted/pending/corrected entries and never combine currencies without policy. |
| E09-01 | S15 | South Africa sandbox adapter creates a payment attempt without storing card data; browser return is not proof of payment. |
| E09-02 | S15 | Signed webhook/event IDs are verified and deduplicated; forged, duplicate or out-of-order callbacks cannot double-post. |
| E09-03 | S16 | Provider settlement, payment attempts and contributions reconcile; exceptions and retry states are visible to finance. |
| E09-04 | S16 | Refund/failed-payment path uses separate authorized records, reversal and provider evidence; no destructive edit of donation history. |
| E09-05 | TBD after South Africa pilot | Mozambique gateway, currency, receipts and settlement mapping pass local finance/legal review and contract tests before online launch. |
| E09-06 | Unscheduled | Third-country gateway, currency, receipts and settlement mapping pass equivalent review and tests before online launch; country order remains open. |
| E09-07 | Unscheduled | Fourth-country gateway, currency, receipts and settlement mapping pass its own finance/legal review and contract tests before online launch. |
| E10-01 | S8 | Purpose/channel-specific templates, consent and suppression rules prevent unapproved or out-of-scope messages. |
| E10-02 | S9 | Email/SMS adapters send invitations and service messages from outbox; retries/dead letters do not roll back membership decisions. |
| E10-03 | S17 | WhatsApp adapter uses approved provider/templates/consent rules; delivery status, failure and suppression are tested. |
| E10-04 | S29 | Authorized bulk campaign preview, audience snapshot, approval/cancel window and opt-out prevent cross-scope sends. |
| E11-01 | S18 | Registered device holds minimal encrypted offline operations with unique client IDs and visible pending count after restart. |
| E11-02 | S18 | Sync API validates account, device, branch and operation ID; replay creates one business transaction and returns stable result. |
| E11-03 | S19 | User sees accepted/rejected/conflicted items and recovery/export path; queue never silently drops a rejected write. |
| E11-04 | S19 | One branch pilots offline giving on controlled devices; duplicate/reconnect/outage tests and support feedback determine expansion. |
| E12-01 | S2 | Inventory legacy schema, source counts, credentials risk and backup/restore approach without connecting to live data absent authorization. |
| E12-02 | S10 | Versioned source→target maps cover members, Identity links and finance; unknown status/identity is quarantined for review. |
| E12-03 | S21 | Full dry-run ETL on isolated snapshot records per-row exceptions, time, rollback and repeatability. |
| E12-04 | S22 | Finance totals by branch/period/currency, member counts, account links, sample documents and access denials reconcile; named South Africa UAT signs off. |
| E12-05 | S23 | South Africa pilot-branch cutover follows approved freeze/backup/go-no-go/monitoring/rollback plan and post-launch support window. |
| E12-06 | TBD after South Africa pilot | Mozambique pilot passes its own data/privacy/payment/UAT gate and measured support criteria. |
| E12-07 | Unscheduled | Third-country pilot passes the same independent gate; unresolved country-specific requirements block only that pilot. |
| E12-10 | Unscheduled | Fourth-country pilot passes its own data/privacy/payment/UAT gate; a prior country's sign-off does not transfer. |
| E12-08 | S21 | Role-based training, UAT scripts and support contacts are rehearsed with representative pilot branches. |
| E12-09 | S26 (directional) | South Africa branch-onboarding checklist and first measured expansion wave use per-branch data/access/support evidence; remaining waves become separately sized tickets. |
| E13-01 | S20 | Document metadata and bytes live in SQL/private object storage respectively; nothing sensitive is served from `wwwroot`. |
| E13-02 | S20 | Upload validates size/type/signature and download reauthorizes owner/classification; wrong-branch and malicious files are denied. |
| E13-03 | S21 | Existing media is scanned/classified and mapped to public/private destination with checksum and exception list. |
| E13-04 | S31 | One-step approval coordination records assignee, deadline, delegation and decision; stale/duplicate decisions do not reapply domain state. |

### E14–E18 — forecast expansion after the core rollout

| Story | Sprint | User outcome / minimum acceptance evidence |
|---|---:|---|
| E14-01 | S27 | Coordinator assigns eligible buddy within branch/capacity policy; reassignment preserves history and excludes pastoral notes. |
| E14-02 | S27 | iMihlangano schedule and effective-dated allocation work beyond fixed weekdays; private venue visibility is enforced. |
| E14-03 | S28 | Attendance handles identified/anonymous counts and short-lived QR; duplicate or offline-replayed check-in posts once. |
| E14-04 | S29 | Event registration, capacity/waitlist and attendance are separate; paid tickets are commercial payments, not giving. |
| E14-05 | S30 | Ministry membership and volunteer shift assignment enforce scope/prerequisites, conflict rules and substitution history. |
| E15-01 | S31 | Pastoral classification, explicit case grants, break-glass policy and read audit are approved and denial-tested before case data is enabled. |
| E15-02 | S32 | Restricted case notes/tasks and referral states exclude narratives from general search, logs, messages and exports. |
| E15-03 | S32 | Prayer visibility and separate publication/testimony consent prevent confidential or anonymous requester disclosure. |
| E15-04 | S33 | Guardian/child access, safeguarding escalation and photo consent pass country-specific policy and denial tests. |
| E16-01 | S26 | Branch/circuit operational dashboard uses scoped, indexed read models with freshness and original-currency labels. |
| E16-02 | S33 | Executive country/denomination aggregates suppress small cells and never expose pastoral narratives or cross-scope member detail. |
| E16-03 | S34 | Large export is asynchronous, scoped, purpose-logged, expiring and auditable; ordinary user queries remain responsive. |
| E16-04 | S34 | Optional engagement version explains factors and excludes giving/pastoral data; no automated adverse decision uses it. |
| E17-01 | S35 | Store catalogue, effective prices and branch stock movements prevent unreasoned or unauthorized adjustment. |
| E17-02 | S36 | Cart/order snapshots price, tax/currency and SKU; concurrent last-item purchase cannot oversell without approved backorder policy. |
| E17-03 | S37 | Commercial checkout, reservation expiry and fulfilment remain distinct from giving and reconcile provider events. |
| E17-04 | S37 | Return/refund transitions and stock restoration are linked and auditable; historic order terms remain readable. |
| E18-01 | S35 | Mobile product case, API/auth and device-risk design are approved against web/PWA usage evidence; no duplicate backend. |
| E18-02 | S38 | Native pilot supports secure sign-in/revocation and only approved member/self-service flows. |
| E18-03 | S38 | Native member profile/giving screens use the same server-side self boundary and accessibility rules as web. |
| E18-04 | S39 | Mobile offline operation replay uses existing IDs/conflict contract; device loss and queued-data recovery are tested. |
| E18-05 | S40 | Limited mobile pilot passes store/privacy/security review, telemetry and support readiness before general release. |

## Sprint-by-sprint execution sequence

An exit statement is a demo/test outcome, not permission to deploy. Production go/no-go is separate.

| Sprint | Stories | Demonstrable exit / dependency |
|---:|---|---|
| S1 | E01-01/02, E02-01 | Decision owners named; secrets remediation owned; CI and synthetic baseline tests run. |
| S2 | E01-03/04, E12-01 | Dangerous routes denied; source inventory and privacy questions known. **Gate G0:** no unowned critical risk. |
| S3 | E02-02, E03-01 | Modular conventions and org hierarchy compile with migration/architecture tests. |
| S4 | E02-03, E03-02/03 | Staging/restore POC and organization crosswalk demonstrated. |
| S5 | E04-01/02 | Invite/link model and scoped grants tested. |
| S6 | E04-03/04 | Cross-branch/member denial, MFA and revocation tested. |
| S7 | E02-04, E05-01/02 | Branch admin can create a draft; outbox worker retries independently. |
| S8 | E05-03/04, E10-01 | Approval/rejection and activation guard tested; messaging rules ready. |
| S9 | E10-02, E06-01 | Approve → invite → login → own profile works. **Gate G1:** internal vertical-slice UAT. |
| S10 | E05-05, E06-03, E12-02 | Transfer/change requests preserve history; migration mapping reviewed. |
| S11 | E07-01/02 | Published curriculum and progress evidence demonstrated. |
| S12 | E07-03 | Completion/exception activates only eligible member. **Gate G2:** membership-policy sign-off. |
| S13 | E08-01/02 | Scoped branch batch capture and immutable post demonstrated. |
| S14 | E08-03/04, E06-02 | Correction, basic receipts/reconciliation and own giving history. **Gate G3:** finance UAT on synthetic data. |
| S15 | E09-01/02 | South Africa sandbox checkout and forged/duplicate-webhook tests. |
| S16 | E09-03/04 | Settlement exceptions and refund/correction path demonstrated. |
| S17 | E10-03, E02-05 | WhatsApp plus email/SMS failure/retry monitoring. **Gate G4:** provider contract/security test. |
| S18 | E11-01/02 | Offline queued operation replays once on reconnect. |
| S19 | E11-03/04 | Branch pilot shows pending/rejected recovery; expand only after evidence. |
| S20 | E13-01/02, E06-04 | Private file upload/download and own-document denial tests. |
| S21 | E12-03/08, E13-03 | Full isolated migration rehearsal, media exception list and pilot training. |
| S22 | E12-04, E01-05, E02-06 | Reconciliation, UAT, security, load and restore evidence. **Gate G5:** South Africa go/no-go. |
| S23 | E12-05 | South Africa pilot-branch cutover and hypercare only if G5 passes. |
| S24 | Re-baseline | Former second-country slot withdrawn; use South Africa pilot evidence to size Mozambique separately. |
| S25 | Re-baseline | Former third-country slot withdrawn; no other-country build or pilot committed. |
| S26 | E12-09, E16-01 | Measured South Africa branch expansion and scoped operational reporting; re-baseline remaining rollout. |
| S27 | E14-01/02 | Buddy and group assignments with capacity/history checks. |
| S28 | E14-03 | Sunday/group attendance, QR and replay tests. |
| S29 | E14-04, E10-04 | Event registration and controlled bulk communication. |
| S30 | E14-05 | Ministry and volunteer roster UAT. **Gate G6:** community release. |
| S31 | E15-01, E13-04 | Pastoral access policy and one-step approvals signed off before sensitive case entry. |
| S32 | E15-02/03 | Restricted cases and prayer visibility pass sensitive-data tests. |
| S33 | E15-04, E16-02 | Safeguarding and aggregate reporting UAT. **Gate G7:** sensitive-module release. |
| S34 | E16-03/04 | Audited exports and optional engagement model reviewed for privacy/bias. |
| S35 | E17-01, E18-01 | Store inventory foundation; mobile go/no-go based on web evidence. |
| S36 | E17-02 | Order reservation and price snapshot tests. |
| S37 | E17-03/04 | Store payment, fulfilment, return/refund UAT. **Gate G8:** commerce release. |
| S38 | E18-02/03 | Native pilot login and self-service, no broader feature parity promise. |
| S39 | E18-04 | Mobile sync, device-loss and conflict tests. |
| S40 | E18-05 | Limited mobile pilot, support, telemetry and release decision. **Gate G9.** |

## Decisions and release gates

| Needed by | Owner / decision | Consequence if late |
|---|---|---|
| Before S3 | Sponsor/product: `DEC-001/002/007/008` country order, capacity, hosting/data location, budget/NFR; technical steering group reviews proposed ADRs. | Continue synthetic foundation work only; no host/country commitment. |
| Before S7–S8 | Church leadership: `DEC-003/004/005/006` approval, activation, household access, transfer/change authority. | Do not close policy-dependent workflow or portal stories. |
| Before S13–S17 | Finance: `DEC-009/010`; information officer/communications: `DEC-011`. | Sandbox work may continue; no real payments, receipts or messages. |
| Before S18 | Operations/security: `DEC-012` device ownership, maximum offline period and shared-device handling. | No branch offline pilot. |
| Before S22/S23 | Information officer/South Africa counsel: `DEC-007/015`, processor contracts/retention and legal review; sponsor/ops approve RTO/RPO. | No South Africa production cutover. |
| Before S31 | Pastoral/safeguarding/legal: `DEC-013/014`. | Do not enable pastoral or children's records. |
| Before S34 | Church leadership/information officer approve any engagement factors and permitted uses. | Skip E16-04; ordinary reports can still ship. |

**Gate G0** proves the team can build safely. **G1** is the first complete member vertical slice. **G2–G4** approve membership, finance and providers separately. **G5** uses the [release-readiness checklist](release-readiness.md): reconciled migration, approved country policy, passed denial/security tests, restore/load evidence, trained operators and rollback trigger. G6–G9 are independent later-module releases; a completed sprint alone does not authorize production. Every later country, including the newly identified fourth, repeats the applicable G5 checks rather than inheriting South Africa's approval. Wider branch waves need their own onboarding evidence; they are not implied by pilot sign-off.

## Execution rules and next actions

1. Create tracker issues from these IDs, link each to its source `REQ-*` or module specification, owner, dependency, test case and UAT evidence. The product owner owns ordering; technical lead owns sequencing; domain approvers own policy.
2. At S1 kickoff, confirm actual team availability and the first country's pilot branches; assign decision owners/dates and credential rotation. Do not put secrets or real member/finance data in the tracker.
3. Refine only the next two sprints to implementation-ready stories (actor, scope, success/failure, data classification, permission, test). Split oversized stories before commitment. Maintain a visible decision/impediment board.
4. Demo working, tested behavior at every sprint review to branch/country representatives; record UAT feedback and revise the forecast. Use a retrospective and capacity data, not the number of planned story IDs, to set the next commitment.
5. A story is done only when code/migrations (where applicable), server authorization, negative tests, API/OpenAPI contract, audit/monitoring, accessibility, documentation and deployment/rollback impact are reviewed. No happy-path-only closure. Use synthetic or approved de-identified test data.
6. At each gate, record sponsor/product, finance, security/information officer and operations decisions with evidence. If blocked, continue safe independent stories; do not infer permission for production DB access, payments, messaging, hosting changes or legal policy.

Source documents: [requirements](../product/requirements-and-backlog.md), [business rules](../product/business-rules-and-workflows.md), [open decisions](../product/decision-register.md), [permissions](../architecture/permissions-matrix.md), [module specifications](../modules/), [test strategy](test-strategy.md), [migration plan](migration-and-cutover.md), [threat model](../security/threat-model.md) and [architecture blueprint](../architecture/church-management-platform-blueprint.md).
