# S3 agent handoff — preparation only

Status: **historical handoff draft from the previous repository**, 16 September 2026. Re-plan its stories against the new clean solution; do not import legacy application code. Its old S1–S2 prerequisites and Gate G0 evidence do not establish a current delivery gate for this repository.

## Entry conditions

- An authorized owner has rotated/revoked exposed database and bootstrap administrator credentials in affected environments; evidence is recorded outside the repository without copying values.
- The integrated S1–S2 build, synthetic tests, dependency audit, route/CSRF inventory and CI/secret-scan result are reviewed. Critical findings are fixed or have a named, time-limited risk decision. An unrun scan is not a pass.
- The Church executive/product owner confirms South Africa-only current build/pilot scope, Mozambique next and Eswatini/Swaziland plus Zimbabwe unscheduled, assigns decision owners and dates, and approves or explicitly defers the proposed foundation ADRs. The core hierarchy remains country-neutral; do not implement other-country configuration or providers now.
- No live database or real member records are used. Migration and schema work uses a disposable database and synthetic fixtures only.

## Proposed three-agent sequence

| Agent | Bounded assignment | Owned output / acceptance |
|---|---|---|
| Lead/integrator | Agree E02-02/E03-01 contracts, migration naming/ownership, `ChurchId` invariant and module boundaries before parallel edits. Review ADR status and integration. | Small accepted implementation slice, integrated build/tests, no unrelated rewrite; one owner for shared `ApplicationDbContext` and migrations. |
| Engineering foundation agent | Introduce minimal modular conventions and versioned API pattern without moving the whole MVC application. | Boundary tests plus a documented pattern that a second module can follow. Do not edit organization migrations or shared EF configuration. |
| Organization agent | Implement a Church → country → circuit → branch hierarchy that supports future countries, but configure/test **South Africa only** for the current product slice, with valid-parent and cycle rules. | EF configuration/migration and synthetic tests for invalid hierarchy, orphan prevention and country-neutral extensibility. Do not seed or implement Mozambique, Eswatini or Zimbabwe configuration/providers yet, or invent country-specific legal, payment or membership policy. |

The lead sequences any shared files (especially `ApplicationDbContext`, project files and migrations) and performs final integration. If both workers need the same file, the lead assigns one owner; do not make concurrent edits to it. Add independent review/testing capacity only after the first vertical slice is stable. The G0 check and technical steering-group approval remain human decisions, not agent self-certification.
