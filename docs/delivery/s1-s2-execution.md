# S1–S2 execution and source inventory

Status: **historical record of the previous repository**, 16 September 2026. It is not the execution status or source inventory of this new solution. The [sprint plan](sprint-delivery-plan.md) remains a forecast and must be re-baselined for South Africa-only implementation, Mozambique next and two other unscheduled countries. No old application code is being moved into the new repository.

Execution model for this pilot: **three concurrent Codex agents total**—a lead/integrator, a security-baseline implementer, and a CI/synthetic-test implementer. The lead owns planning and migration/privacy inventory. The two implementers have disjoint file ownership; the lead reviews and runs final integrated verification. This does not replace human product, Church, finance, information-officer, legal or operations approvals. Start S3 only after S1–S2 evidence and Gate G0 risks are reviewed.

## Scope and ownership

| Story | Current execution boundary | Evidence or dependency still needed |
|---|---|---|
| E01-01 | User identified South Africa, Eswatini (Swaziland), Mozambique and Zimbabwe as long-term countries, then clarified South Africa-only build/pilot now and Mozambique next. Estimates: 10,000 members now and in three years plus 6,000 giving transactions/month at launch. | Product owner and Church executive confirm country timing beyond Mozambique, peak concurrency/transaction rate, document volume, decision owners and target dates in [DEC-001/002](../product/decision-register.md). |
| E01-02 | Remove deployable bootstrap secrets and establish secret injection without using old values. | Authorized owner must rotate exposed database/default-admin credentials in affected environments and record evidence outside this repository. No agent can claim rotation from a code edit. |
| E02-01 | Establish isolated build/test/CI baseline using synthetic data. | CI run evidence and tests are needed; no run may use the committed connection string. |
| E01-03 | Inventory routes and apply default-deny, anti-forgery and scope tests. | Current code lacks the target Church/country/circuit/branch permission model; S2 can secure the legacy surface, but full resource-scope enforcement depends on E03–E04. |
| E01-04 | Draft country-specific data-flow questions from the current code paths. | Information officer and qualified country counsel must determine lawful basis, residency, processors, retention and child/pastoral handling. See [privacy inventory](../security/privacy-and-data-inventory.md). |
| E12-01 | Repository-only source schema and media inventory below. | Row counts, quality checks, database version, restore evidence and source credentials cannot be verified without an explicitly authorized isolated snapshot. |

## Repository-only legacy source inventory

The source is `ChgManagementSystem/`, an ASP.NET Core MVC/Identity application using `ApplicationDbContext` and EF migrations. Its known model groups are:

| Group | Current code evidence | Migration question |
|---|---|---|
| Organization | `Circuit`, `Branch` (`CircuitId`), `BranchLeader` (`BranchId`) | No country or `ChurchId` in these models. Determine stable source IDs and ownership/crosswalk; do not infer country from names. |
| Membership and accounts | `Member` (`BranchId`, `IsActive`, `HasSystemAccess`); ASP.NET Identity tables | `HasSystemAccess` is not a verified account-to-member link. Do not auto-link by name/email or interpret `IsActive` as approved journey completion. |
| Giving | `TitheRecord` (`MemberId`, month/year, promised/paid amount, payment date); `MonthlyOffering` (`MemberId`, `OfferingTypeId`, amount, month/year); `OfferingType` | Confirm meaning of tithe versus offering, original branch/currency, payment/posting status, duplicates and reconciliation with finance. Current models do not carry currency or an immutable posting ledger. |
| Public website/media | `NewsPost`, `WebsiteSettings`, `GalleryAlbum`, `GalleryPhoto`; branch/leader image paths | Classify every file as public/private and verify checksums before migration. Existing `wwwroot/uploads` content is not evidence that private files are safe to publish. |

No source row counts, database contents, null/duplicate rates, referential-integrity results, backup/restore proof or identity-link validation are established by this repository inspection. The [migration plan](migration-and-cutover.md) requires these from a read-only, isolated snapshot after authorization. Keep an exception list rather than silently assigning unknown country, branch, member status, currency or account links.

## Current data flows requiring country assessment

1. Browser and staff forms submit member, branch, leader, giving and public-site content to MVC controllers, which write through EF Core to SQL Server. Determine hosting and backup locations, administrator access, retention and country-to-country visibility before any live deployment.
2. The application can create Identity users for members. Account creation and member linkage need a verified invitation design before self-service; current `HasSystemAccess` alone is insufficient proof of identity.
3. Member and finance controllers generate downloadable reports. These need record-level authorization, purpose/audit rules and export retention controls before use with real data.
4. Several controllers save uploaded images under `wwwroot/uploads`. Public website media and private member evidence must be classified and separated before migration; do not move or delete existing uploads during this inventory.
5. No payment, SMS, email or WhatsApp provider adapter was found in current C# source. Future provider data flows remain design questions, not approved processors.

Before its own pilot, each country needs an information-officer and counsel review using the [country-readiness matrix](../security/country-readiness.md), including responsible entities, legal basis, residency/transfer, processor contracts, retention, data-subject requests and incident handling. South Africa-only build priority does not authorize a South African live pilot before Gate G5; Mozambique is a later country gate, not current implementation scope.

## Interim security and verification position

The current implementation pass has removed the deployable database connection value, removed hard-coded bootstrap administrator creation, disabled the legacy member-account shortcut and public self-registration, added an authenticated fallback, restricted sensitive legacy controllers to administrator access, and applied MVC antiforgery validation. Public registration links were removed from the login experience. These are **interim safety controls**, not the E03–E04 organizational permission system. In particular, the existing admin role is not scoped to a country, circuit or branch.

The removed database credential remains in Git history and must be treated as compromised until an authorized owner rotates/revokes it in each affected environment. Existing accounts, public upload paths and any copies of sensitive media need a separate review. Do not use this code with real member or finance data to infer that the S2 authorization gate has passed.

The [legacy route and upload audit](../security/legacy-route-and-upload-audit.md) found and corrected two state-changing GETs: branch-leader deletion and first-time website-settings creation. The audit also found unvalidated image uploads into public `wwwroot/uploads`; 19 tracked JPEG assets were counted but not opened or classified. Existing and new public media need an owner-led publication/consent decision. No private-media safety is implied by the temporary `Admin` upload boundary.

## Gate G0 evidence and ownership

| Evidence | Current state | Who closes it |
|---|---|---|
| Deployed credential rotation/revocation | **Open.** Current source no longer embeds the connection value or bootstrap administrator password, but Git history and affected environments remain. | Authorized database/operations owner; record evidence outside repo, no values in tickets. |
| Local engineering baseline | **Passed locally.** Build, synthetic tests and strict dependency audit; see verification below. | Technical lead reviews integration; QA expands coverage as risk warrants. |
| GitHub CI and full-history secret scan | **Not run.** Workflow configured; local approved scanner unavailable. Do not mark clean by assumption. | Repository owner/CI operator runs in a controlled environment and reviews findings without sharing secret values. |
| Legacy route and CSRF containment | **Partly addressed.** Default-deny/admin boundary and two unsafe GET fixes have isolated HTTP regression coverage; organizational/record scope remains absent. | Security lead and QA review [audit](../security/legacy-route-and-upload-audit.md); E03–E04 deliver real organizational scope. |
| Public upload risk | **Open.** Legacy writers lack image validation and bytes are served statically. | Product/security owner chooses temporary disablement or approved image gate before any public deployment; E13 handles private storage. |
| Country/privacy and migration evidence | **Draft only.** South Africa-only build with Mozambique next; repository-only inventory, no isolated source snapshot or legal approval. | Product owner, information officer, country counsel, database/operations owners. |

**Gate G0 is not passed.** Do not move to an operational pilot or represent S3 as approved merely because local tests pass. Safe, synthetic design preparation may continue within the decisions still open.

Local engineering verification completed after the S2 security changes: test-project restore, Release solution build, **26/26 synthetic tests**, and strict low-severity transitive NuGet audit passed. The build still reports 33 existing nullable warnings. The initially reported NU1901 on transitive `NuGet.Packaging` 6.12.1 was resolved in the new test project by a private 6.12.5 pin; no warning was suppressed. See [test instructions](../../tests/README.md). The GitHub CI workflow and full-history secret scan are configured but **have not run on GitHub or locally**; no approved local scanner/container executable was available, and historical committed-secret findings remain expected. These local results do not close E01-02 credential rotation, E01-03 organizational scope or Gate G0. Record CI/secret-scan evidence and named risk decisions before marking the stories done.
