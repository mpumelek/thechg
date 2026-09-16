# ADR 0001: Modular monolith

Status: Proposed. Date: 2026-09-16. Owner: Solution architect. Approver: Technical steering group — TBD.

## Context

The Church now states four countries, about 25 circuits and 175 branches. The team is Microsoft-oriented and the existing codebase is one ASP.NET Core MVC application. Domain breadth is high, but independent deployment and scaling of modules are not yet demonstrated needs.

## Decision

Use one ASP.NET Core web/API deployable plus an independent background worker. Organize business features as modules with domain/application/infrastructure/contracts, module-owned schemas and architecture tests. Modules communicate through contracts and domain/outbox events, not direct cross-module table writes.

## Alternatives considered

Microservices: stronger runtime isolation but disproportionate operations, distributed transactions and observability burden. Unstructured MVC monolith: fastest short-term but preserves current controller coupling. Fully separate databases per module: premature migration and reporting complexity.

## Consequences

Simpler deployment, local transactions, and clear migration from the current repository. Module boundaries require review and automated enforcement. Extract a service only when scale, security, reliability or independent release cadence proves a need.
