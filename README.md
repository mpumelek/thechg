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

The current code is a compilable foundation, **not** a functioning church management system. Authentication, SQL Server persistence, membership, giving, payments, notifications and reporting still require implementation and approval of the relevant policies and providers.

## Build

Install the .NET 10 SDK, then run:

```powershell
dotnet restore TheChg.slnx
dotnet build TheChg.slnx --no-restore
dotnet test TheChg.slnx --no-build
```

The Web project uses the standard MVC template and has no production data connection. The Worker is a separate process placeholder and has no live jobs or provider credentials.

## Planning documents

Start at [docs/README.md](docs/README.md). These documents were transferred from the earlier planning repository. They include useful requirements and architecture drafts, plus some old-code audits that are retained only for historical context. The [agent guide](agent.md) defines the current repository rules.
