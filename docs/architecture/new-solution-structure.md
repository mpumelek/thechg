# New solution structure

Status: implementation baseline, 16 September 2026. This document describes the new repository's project boundaries and first organization slice, not completed product functionality.

The fresh `.NET 10` solution is a modular-monolith foundation with two deployment processes: an ASP.NET Core web host and a separate worker. Both will compose application use cases and infrastructure adapters. The organization module now has a SQL Server EF Core mapping and first migration; the worker has no registered jobs or database adapter yet.

```text
TheChg.Web ────────────────┐
TheChg.Worker ─────────────┤
                           ▼
                   TheChg.Infrastructure
                           │
                           ▼
                   TheChg.Application ─────► TheChg.Contracts
                           │
                           ▼
                      TheChg.Domain
```

The arrows represent allowed compile-time dependencies; Domain and Contracts are independent. Infrastructure can implement ports defined by Application. Hosts register adapters and handle transport concerns, not business decisions. Domain modules and application use cases should be organized by bounded context as they are built; do not create an entire empty project for each future module.

The current solution has only the first organization model and migration. It has no authentication, authorization policy, outbox, payments, notifications, or member portal. The `/health/live` endpoint reports only web-process liveness, not database or provider readiness. No worker jobs are registered. Do not deploy this scaffold as a functioning church system.

The [delivery plan](../delivery/sprint-delivery-plan.md) and [decision register](../product/decision-register.md) guide the next slices. South Africa is the active build scope. Country expansion remains a later decision and should not be pre-configured with unapproved rules.
