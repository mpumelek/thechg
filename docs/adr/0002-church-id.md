# ADR 0002: Include ChurchId from day one

Status: Proposed. Date: 2026-09-16. Owner: Database architect. Approver: Technical steering group — TBD.

## Context

The initial product serves one denomination, not a multi-church SaaS. Future commercialization is possible but uncertain. Adding ownership keys later would require touching nearly every table and API.

## Decision

Include `ChurchId` on church-owned operational records and enforce it in data access, uniqueness constraints, audit and API context. The server resolves the active church; ordinary clients cannot choose an arbitrary `ChurchId`. There is one initial church row. Organizational scope still controls country/circuit/branch access.

## Alternatives considered

Omit the key until SaaS exists: lower upfront work, expensive later migration. Full tenant isolation now: premature operational complexity.

## Consequences

Small storage/index cost and more explicit ownership. Query filters are defense-in-depth, not authorization. Future SaaS would still require provisioning, billing, isolation testing and operations work; this ADR does not claim the system is already multi-tenant.
