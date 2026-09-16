# ADR 0003: Identity separated from membership; scoped permissions

Status: Proposed. Date: 2026-09-16. Owner: Security architect. Approver: Church leadership and security lead — TBD.

## Context

Members cannot self-register as official Church members. Staff authority depends on country/circuit/branch and some records, especially pastoral and minors, need tighter access. Global role checks in the current codebase are insufficient.

## Decision

Use ASP.NET Core Identity initially for accounts and separate `ApplicationUser` from `Member`, linked by verified `MemberUserLink`. Invite users after governance eligibility. Roles bundle permissions; dated assignments scope permissions to an organizational unit. Every resource check also evaluates record restrictions. Privileged accounts require MFA. Public registration never creates a member.

## Alternatives considered

Global role-only authorization: simpler but cannot express branch/record scope. One `Member` as login identity: conflates governance and authentication. External IdP immediately: useful later, but adds procurement and integration before requirements are clear.

## Consequences

More authorization code and matrix tests, but clear least-privilege behavior and a safe member portal. Mobile authentication may require a standards-based token service later. Changing role labels does not require changing domain code.
