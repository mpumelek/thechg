# Open decision register

Status: Open. Owner: Product owner. Each decision needs an owner, target date, evidence, final outcome and a link to an ADR or business rule where applicable. `TBD` is intentional; no assumption below authorizes implementation of an unapproved policy.

| ID | Decision needed | Decision owner | Blocks |
|---|---|---|---|
| DEC-001 | Confirm stated four-country long-term scope; current build/pilot is South Africa only, Mozambique next, while Eswatini/Swaziland and Zimbabwe timing remains open | Church executive | Country readiness, provider choice, sprint re-baseline |
| DEC-002 | Current/three-year member counts, concurrent users and transaction volumes | Product owner | NFR targets, capacity sizing |
| DEC-003 | Who may approve member registration, with what quorum/appeal path? | Church leadership | Approval workflow |
| DEC-004 | What exactly graduates a new member to active status? | Church leadership | Curriculum rules |
| DEC-005 | May household/guardian users see another person's profile or giving? | Church leadership + information officer | Portal authorization |
| DEC-006 | Who approves branch transfers and profile changes? | Church leadership | Workflow rules |
| DEC-007 | Hosting/data/backup location and cross-border legal assessment | Information officer + legal + operations | Production deployment |
| DEC-008 | RTO, RPO, uptime, latency, support hours and budget | Sponsor + operations | Hosting/DR design |
| DEC-009 | Payment gateway and settlement account per country | Finance leadership | Online giving |
| DEC-010 | Refund/adjustment authority and country receipt/accounting policy | Finance leadership | Finance release |
| DEC-011 | Communication providers, languages, lawful basis and opt-out rules per country | Information officer + communications | Messaging release |
| DEC-012 | Branch offline-device ownership, shared-device policy and maximum offline period | Operations + security | PWA sync design |
| DEC-013 | Pastoral case classifications and emergency-access authority | Pastoral leadership + security | Pastoral module |
| DEC-014 | Children's participation, guardian consent and photo-publication policy | Safeguarding lead + legal | Youth/Sunday School |
| DEC-015 | Data retention and deletion schedule by category/country | Information officer + legal | Privacy implementation |

Review this register at every phase gate. Decisions about architecture that are hard to reverse also require an [ADR](../adr/README.md).

Scope input recorded 16 September 2026: the user named four countries, then clarified **build for South Africa only now; Mozambique next**. Eswatini/Swaziland and Zimbabwe remain future, unscheduled. No country-specific provider, legal position or go-live approval was supplied.

Capacity input recorded 16 September 2026: approximately **10,000 members now and 10,000 expected in three years**, with **6,000 manual-plus-online giving transactions per month at launch**. Peak concurrent users, document volume, country distribution and peak transaction rate remain TBD; DEC-002 stays open until the product owner confirms sizing evidence.
