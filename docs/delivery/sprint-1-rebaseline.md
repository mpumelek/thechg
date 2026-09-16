# New-repository Sprint 1: organizational foundation

Status: engineering baseline implemented and [first GitHub CI run passed](https://github.com/mpumelek/thechg/actions/runs/35125561701), 16 September 2026; Church policy and release gates remain open. This supersedes the old S1–S2 execution assumptions, not the product backlog or the Church's approval gates. Scope is South Africa only. No old source code or live data is imported.

## Sprint outcome

A clean, buildable .NET 10 solution has repeatable CI, a SQL Server organization schema and an internal use case for creating South African country → circuit → branch units. No public organization write endpoint is exposed until identity and scoped authorization exist. The first migration was reviewed and applied only to a newly named local validation database, `TheChg_Sprint1Validation_20260916_92C1C6`; no member or Church records were seeded. Shared environments remain untouched.

## Work and evidence

| Item | Acceptance evidence |
|---|---|
| Engineering baseline | Restore, Release build and automated tests pass in GitHub Actions; no SQL Server or credentials are needed for CI tests. |
| Domain hierarchy | Country has no parent; circuit requires country; branch requires circuit; children inherit ChurchId, country, timezone and currency. IDs, names and codes are validated. |
| Persistence | Explicit EF Core mapping, unique sibling/country codes, same-Church parent foreign key, restrictive deletes and initial SQL Server migration. Automated persistence tests use in-memory SQLite and synthetic records. |
| Development setup | LocalDB-only example connection on Windows; other environments inject `ConnectionStrings__ChurchDatabase`. No automatic migration or production seed. |
| Security boundary | No administrative API or UI is enabled before identity, scoped grants and auditing are implemented. |

The transferred story IDs map to this new baseline as follows. `E02-01` (build/test CI) and the solution-boundary portion of `E02-02` are implemented and verified by the first GitHub run. `E03-01` is partial: the hierarchy and database guards exist, but authorized administration, audit and approval of the real Church registry remain. The old `E01-02`, `E01-03` and `E03-03` legacy-code/data tasks are not carried forward as application-code work. Any credential rotation or data import for the old system requires its own authorized workstream.

## Out of scope and next sprint

The actual denomination name and first circuit/branch registry still need Church-owner confirmation. No member, account, giving, payment, messaging, leadership assignment or production deployment work is done here. The next slice is staff identity and explicit country/circuit/branch authorization, followed by approved organization administration endpoints and audit. Re-plan old E01/E02/E03 story IDs against this clean baseline before claiming their historical acceptance evidence.
