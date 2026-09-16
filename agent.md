# Working guide for coding agents

This repository is a clean-room start for The CHG church management platform. Do not import application code, database credentials, configuration, migrations, or test fixtures from the previous repository.

## Scope and structure

- Build for South Africa first. Mozambique is the next planned country; Eswatini and Zimbabwe are future options, not current delivery commitments.
- One denomination, about 10,000 members and approximately 6,000 giving transactions per month at launch. The target organizational model is denomination → country → circuit → branch.
- `TheChg.slnx` targets .NET 10. `src/` contains Domain, Contracts, Application, Infrastructure, Web, and Worker projects; `tests/` contains their initial test projects.
- This is not a production-ready implementation. The organization domain, SQL Server EF Core mapping and first migration exist; there is no membership, authentication, payment, messaging, or background-job behavior yet. No organization write endpoint is exposed.
- [Documentation index](docs/README.md) contains transferred planning drafts. Documents about the previous codebase are historical only. Do not mistake draft OpenAPI, runbooks, or sprint plans for implemented features or approved Church policy.

## Working rules

1. Inspect `git status` and preserve unrelated changes. Work in the smallest coherent vertical slice.
2. Before implementing behavior, read the relevant module specification, [business rules](docs/product/business-rules-and-workflows.md), [permission matrix](docs/architecture/permissions-matrix.md), and [decision register](docs/product/decision-register.md). `TBD` requires an owner decision, not a developer guess.
3. Keep project dependencies inward: Domain and Contracts stand alone; Application depends on Domain and Contracts; Infrastructure implements Application abstractions; Web and Worker are composition roots. Avoid references from inner layers to Infrastructure, Web, or Worker.
4. Keep transport DTOs separate from domain and persistence models. Put authorization and organization-scope checks server-side in use cases, not only in UI navigation.
5. Official membership registration is by authorized staff. Member accounts and records are distinct. A member's self-service data must be resolved from the authenticated account, never from a caller-supplied member ID alone.
6. Historical contributions and branch attribution must be preserved. Posted giving corrections use reversal or adjustment records, not destructive edits.
7. Keep secrets and real personal, payment, children's, or pastoral data out of the repository and test fixtures. Do not contact live services or run production migrations without explicit authorization.
8. Add tests for business rules and denial paths. Run relevant tests and the solution build; report actual results. Do not claim a scaffold endpoint is a working feature.
9. Update documentation and an ADR for consequential structural decisions. Do not silently mark drafts approved.

## Initial implementation sequence

Follow the [new-repository Sprint 1 baseline](docs/delivery/sprint-1-rebaseline.md) and [draft backlog](docs/delivery/sprint-delivery-plan.md). The organization model and isolated migration baseline are first; identity and scoped authorization are next. Payments, member portal, messaging and country expansion follow their approved dependencies and controls.
