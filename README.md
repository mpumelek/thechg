# The CHG Platform

A new, web-first church management platform for one denomination. South Africa is the first delivery country; Mozambique is planned next. This repository was started from an empty GitHub repository and contains **no application code from the previous system**.

## Solution

| Project | Responsibility |
|---|---|
| `TheChg.Domain` | Business entities, value objects and invariants; no framework dependencies |
| `TheChg.Contracts` | Public API and integration contracts, independent of persistence |
| `TheChg.Application` | Use cases, authorization requirements and ports |
| `TheChg.Infrastructure` | Database and external-service adapters, added incrementally |
| `TheChg.Web` | ASP.NET Core MVC web experience and future versioned APIs |
| `TheChg.Worker` | Out-of-process jobs, messaging and payment follow-up |
| `TheChg.*.Tests` | Unit and architecture tests |

The current code is a foundation, **not** a functioning church management system. The South Africa organization model and SQL Server migrations exist, alongside inactive-by-default Identity account storage and a fail-closed scoped authorization evaluator. There is **no login or registration endpoint, member management, persisted permission grant, giving, payment, notification or reporting feature** yet. Organization administration remains closed.

## Build

Install the .NET 10 SDK, then run:

```powershell
dotnet restore TheChg.slnx
dotnet build TheChg.slnx --no-restore
dotnet test TheChg.slnx --no-build
```

The Web project uses the standard MVC template and has no production data connection. The Worker is a separate process placeholder and has no live jobs or provider credentials.

For isolated database setup and migration review, see [local development](docs/operations/local-development.md). CI tests use SQLite and synthetic records, not SQL Server or live church data. The [Sprint 2 plan](docs/delivery/sprint-2-identity-authorization.md) tracks the remaining identity and authorization work.

## Planning documents

Start at [docs/README.md](docs/README.md). These documents were transferred from the earlier planning repository. They include useful requirements and architecture drafts, plus some old-code audits that are retained only for historical context. The [agent guide](agent.md) defines the current repository rules.
