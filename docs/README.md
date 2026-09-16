# Church platform documentation

Status: transferred planning documentation, 16 September 2026. This repository is a **new solution from an empty repository**. The [system architecture blueprint](architecture/church-management-platform-blueprint.md) is directional; none of these documents proves a feature is implemented. South Africa is the only current build target, with Mozambique next.

Legacy-specific material—including the [old dependency upgrade record](operations/dependency-upgrade-2026-09-16.md), [old route/upload audit](security/legacy-route-and-upload-audit.md), [old test baseline](archive/legacy-test-baseline.md), and old-source references in the [S1–S2 execution record](delivery/s1-s2-execution.md)—is retained solely as historical context. It must not guide code migration or describe the new solution's current implementation. The root [README](../README.md) and [agent guide](../agent.md) describe the new codebase.

## How to use this set

1. Church leadership and the product owner approve the [charter](product/charter.md), [business rules](product/business-rules-and-workflows.md), and [requirements](product/requirements-and-backlog.md).
2. Architecture and security leads resolve the [open decisions](product/decision-register.md), [quality targets](architecture/non-functional-requirements.md), [permission matrix](architecture/permissions-matrix.md), [data model](architecture/data-model-and-dictionary.md), and [privacy assessment](security/privacy-and-data-inventory.md).
3. Build one vertical slice at a time from the [module specifications](modules/), keeping [API contracts](api/) and tests in sync.
4. Before production, complete the [release checklist](delivery/release-readiness.md) and rehearse the [operations runbooks](operations/).

Every document is a draft unless marked approved by its named owner. Replace `TBD` with a decision, date, and approver; do not silently treat an assumption as policy. Keep secrets, real member data, payment credentials, and confidential pastoral records out of this repository.

## Index

| Area | Documents |
|---|---|
| Product | [Charter](product/charter.md), [requirements/backlog](product/requirements-and-backlog.md), [business rules/workflows](product/business-rules-and-workflows.md), [glossary](product/glossary.md), [decision register](product/decision-register.md) |
| Architecture | [New solution structure](architecture/new-solution-structure.md), [blueprint](architecture/church-management-platform-blueprint.md), [context/containers](architecture/context-and-containers.md), [data model](architecture/data-model-and-dictionary.md), [permissions](architecture/permissions-matrix.md), [non-functional requirements](architecture/non-functional-requirements.md), [ADRs](adr/) |
| Modules | [Organization/identity](modules/organization-and-identity.md), [membership](modules/membership.md), [new member journey](modules/new-member-journey.md), [buddy and iMihlangano](modules/buddy-and-cell-groups.md), [attendance/events/ministries](modules/community-operations.md), [giving](modules/giving-and-payments.md), [store](modules/store.md), [pastoral/prayer](modules/pastoral-and-prayer.md), [communications](modules/communications.md), [documents/approvals](modules/documents-and-approvals.md), [reporting/engagement](modules/reporting-and-engagement.md) |
| Security | [Threat model](security/threat-model.md), [privacy inventory](security/privacy-and-data-inventory.md), [country readiness](security/country-readiness.md) |
| API/integrations | [API conventions](api/api-conventions.md), [contract draft](api/openapi.yaml), [payments](api/integrations/payments.md), [messaging](api/integrations/messaging.md) |
| Experience | [UX flows/design system](ux/flows-and-design-system.md), [role guides](training/role-guides.md) |
| Delivery | [Sprint 1 baseline](delivery/sprint-1-rebaseline.md), [Sprint 2 identity/authorization plan](delivery/sprint-2-identity-authorization.md), [draft sprint backlog](delivery/sprint-delivery-plan.md), [historical S1–S2 record](delivery/s1-s2-execution.md), [historical S3 agent handoff](delivery/s3-agent-handoff.md), [optional legacy data import](delivery/migration-and-cutover.md), [test strategy](delivery/test-strategy.md), [release readiness](delivery/release-readiness.md) |
| Operations | [Local development](operations/local-development.md), [deployment](operations/deployment-runbook.md), [historical dependency upgrade record](operations/dependency-upgrade-2026-09-16.md), [support](operations/support-runbook.md), [financial procedures](operations/financial-operating-procedures.md), [incident response](operations/incident-response.md), [disaster recovery](operations/disaster-recovery.md) |

## Review convention

Each change that alters church policy needs product-owner and relevant leadership approval. Each architectural change needs an ADR. Each security/privacy change needs the security lead and information officer. Keep approved ADRs append-only; supersede them with a new ADR rather than rewriting history. This follows [Microsoft's ADR guidance](https://learn.microsoft.com/en-us/azure/well-architected/architect-role/architecture-decision-record).
