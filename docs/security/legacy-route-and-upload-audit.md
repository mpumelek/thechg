# Legacy route and upload audit

Status: **historical audit of the previous repository**, 16 September 2026. Source links below intentionally point to files not present in this new repository. Do not interpret the findings as a scan of the new solution or migrate the old code. This is **not** a penetration test, policy approval, or production release sign-off.

## Scope and method

Inspected the 12 source-controlled MVC controllers, the two scaffolded Identity account pages, the [request pipeline](../../ChgManagementSystem/Program.cs), upload writers, and the contents of `ChgManagementSystem/wwwroot/uploads` by aggregate count/extension only. No application was started, no database was accessed, no upload bytes were opened, and no live route was probed. Other Identity UI pages come from the framework package and were **not** individually enumerated or dynamically tested here. The [permission matrix](../architecture/permissions-matrix.md) and [threat model](threat-model.md) are draft design inputs, not evidence that scoped authorization exists.

`Program.cs` currently requires authentication by fallback policy and applies MVC `AutoValidateAntiforgeryToken`; controller attributes below are the additional boundary. `UseStaticFiles()` runs before authentication/authorization, so `wwwroot` files are public by path independently of controller attributes. The class-level `Admin` attributes combine with narrower action attributes: an action annotated `Admin,Deacon` still requires `Admin` because the class requirement also applies. This is restrictive interim containment, not an approved role policy.

## Route inventory

| Controller/page | Anonymous routes | Current protected routes and data | Scope gap |
|---|---|---|---|
| [Home](../../ChgManagementSystem/Controllers/HomeController.cs) | `Index`, `Privacy`, `Error` | None | Home publishes website settings, published news and gallery data; gallery publication consent is not modeled. |
| [NewsPosts](../../ChgManagementSystem/Controllers/NewsPostsController.cs) | `Index`, `Details`; non-admin requests filter `IsPublished` | Create/edit/delete and draft visibility require `Admin` | Global role only; no publisher assignment, publication review or media classification. |
| [GalleryAlbums](../../ChgManagementSystem/Controllers/GalleryAlbumsController.cs) | `Index`, `Details`, `ViewAlbum` | Create/edit/delete require `Admin` | All gallery records are public on these reads; no publication state or consent gate. |
| [GalleryPhotoes](../../ChgManagementSystem/Controllers/GalleryPhotoesController.cs) | `Index`, `Details` | Create/edit/delete require `Admin` | All photo records are public on these reads; no publication state or consent gate. |
| [Branches](../../ChgManagementSystem/Controllers/BranchesController.cs), [Circuits](../../ChgManagementSystem/Controllers/CircuitsController.cs) | None | All list/detail/create/edit/delete require `Admin` | No country/circuit/branch grant or descendant check. Branch images remain public by static URL. |
| [BranchLeaders](../../ChgManagementSystem/Controllers/BranchLeadersController.cs) | None | Create/edit/delete require `Admin`; delete GET is now read-only confirmation and delete POST requires antiforgery | No organizational scope or effective-dated leadership authority; photos remain public by static URL. |
| [Members](../../ChgManagementSystem/Controllers/MembersController.cs) | None | All member list/detail/CRUD/export require `Admin`; legacy `CreateLogin` GET/POST return 404 | No record/branch scope or export audit; mutable `IsActive` and physical delete bypass future workflow. |
| [TitheRecords](../../ChgManagementSystem/Controllers/TitheRecordsController.cs), [MonthlyOfferings](../../ChgManagementSystem/Controllers/MonthlyOfferingsController.cs), [OfferingTypes](../../ChgManagementSystem/Controllers/OfferingTypesController.cs) | None | All finance reads, edits, deletes and exports require `Admin` | No branch/fund/currency scope, self-only giving, immutable posting or export audit. |
| [WebsiteSettings](../../ChgManagementSystem/Controllers/WebsiteSettingsController.cs) | None | `Edit` GET/POST require `Admin`; GET is now read-only, POST creates/updates under antiforgery | One global role; media inputs and public content have no approval/classification gate. |
| [Login](../../ChgManagementSystem/Areas/Identity/Pages/Account/Login.cshtml.cs) | Login GET/POST | Password failures count toward lockout | MFA, rate-limit and session-revocation behavior are not proven by this inspection. |
| [Register](../../ChgManagementSystem/Areas/Identity/Pages/Account/Register.cshtml.cs) | None | GET/POST require `Admin`; creates an Identity account, not an official member | This is not the proposed scoped invitation/link workflow and should not be used to grant branch authority. |

All ordinary MVC writes found in this inventory are POSTs protected by the global MVC antiforgery filter and/or explicit `[ValidateAntiForgeryToken]`. The two GET writes found during inspection were corrected: `BranchLeaders/Delete` no longer removes a row on GET; `WebsiteSettings/Edit` no longer inserts a default row on GET. Dynamic denial tests are still required before claiming E01-03 complete.

## Upload and static-file inventory

| Writer | Destination under public `wwwroot/uploads` | Observed issue |
|---|---|---|
| [Branches/Create](../../ChgManagementSystem/Controllers/BranchesController.cs) | `branches/` | Copies uploaded bytes with client filename extension. |
| [BranchLeaders/Create/Edit](../../ChgManagementSystem/Controllers/BranchLeadersController.cs) | `leaders/` | Same; leader photographs are directly addressable. |
| [GalleryAlbums/Create](../../ChgManagementSystem/Controllers/GalleryAlbumsController.cs) | `gallery/albums/` | Same; public gallery lacks explicit publication transition. |
| [GalleryPhotoes/Create](../../ChgManagementSystem/Controllers/GalleryPhotoesController.cs) | `galleryphotos/` | Same; public gallery lacks explicit publication transition. |
| [NewsPosts/Create/Edit](../../ChgManagementSystem/Controllers/NewsPostsController.cs) | `news/` | Same; unpublishing a post does not remove direct access to its image URL. |
| [WebsiteSettings/Edit](../../ChgManagementSystem/Controllers/WebsiteSettingsController.cs) | uploads root | Same for logo and hero image. |

Across these writers, code inspection found no application-level byte-size cap, extension allowlist, content signature verification, image re-encoding, malware scan, or private staging area. A generated GUID reduces filename collision/guessing but does not validate content or authorize retrieval. `GalleryAlbum`/`GalleryPhoto` edit actions also bind media path properties from the request. Do not treat path obscurity or an `Admin` upload gate as a content-safety control.

The working tree contains **19 tracked `.jpeg` files** in `wwwroot/uploads` (3,266,968 bytes total), including `deacons/` assets without a current writer in the inspected controllers. This audit did not inspect image contents or establish publication consent/classification. Existing or future media in `wwwroot/uploads` is directly served as static content; controller authorization and later record state do not protect it. Do not move/delete these files without a reviewed inventory and migration plan.

## Action boundary and owners

| Priority / workstream | Action and minimum evidence |
|---|---|
| **S2 containment — security lead + engineering** | Run isolated HTTP denial tests for anonymous member/finance/write/export routes, unpublished news direct ID, admin-only registration, and CSRF. Specifically prove branch-leader delete GET makes no change and POST without antiforgery fails; prove website-settings GET makes no row and first valid POST creates one. Keep the `Admin` role-only boundary explicitly temporary. |
| **S2 containment — security/product owner** | Before any public deployment, disable legacy uploads or implement a bounded, reviewed image-only upload gate (size, extension, signature, safe decoding/re-encoding, storage and response controls), then test active-content and oversized-file denial. Review all 19 tracked assets for intended public publication, including child/leader images; no consent is inferred from their presence. Record owner and expiry for any accepted residual risk. |
| **E03 organization** | Model Church → country → circuit → branch and effective dates without hard-coded country branches; migrate legacy IDs with an exception list. This supplies the resource ancestry needed for scope decisions. |
| **E04 identity/authorization** | Replace global `Admin` checks with action + Church + organizational + record policies; separate account/member link, dated assignment, MFA and revocation. Add sibling-branch, guessed-ID, export and delegation denial tests. Current code cannot satisfy E01-03's sibling-branch acceptance criterion merely with role checks. |
| **E05/E08 finance and membership integrity** | Replace legacy direct member status/delete and mutable giving edits with reviewed workflows, linked corrections, audit, currency/branch invariants and self-service account linkage. |
| **E13 private documents and media** | Move sensitive bytes outside `wwwroot`; authorize each download. Introduce explicit public-media publication and consent state, scan/classify legacy media, checksum migrations and exception handling. Country-specific privacy/publication decisions remain with the information officer and safeguarding lead. |

## Verification state and unresolved decisions

This document is based on source inspection and aggregate file enumeration. A separate agent reported passing isolated HTTP regressions for the branch-leader and website-settings GET/POST fixes using synthetic data; final full-suite/build evidence is tracked separately. No live service or database was used for this audit. Static-file reachability is inferred from `UseStaticFiles()` and file location, not from an external probe. No conclusion about the safety, ownership, or consent status of the 19 image bytes is made.

Open decisions include role/approval delegation, child/leader photo publication, retention, country privacy requirements, hosting location and permitted media handling; see the [decision register](../product/decision-register.md) and [privacy inventory](privacy-and-data-inventory.md). No live credentials, production data or external provider were accessed for this audit.
