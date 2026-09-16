# Reporting and member engagement specification

Status: Draft. Owners: Reporting lead, Church executive, information officer. Numerical targets and engagement policy are not approved.

## Reporting tiers

Operational reports answer branch/circuit questions from indexed transactional read models. Executive reports use country/denomination aggregates with small-count suppression where re-identification is possible. Large exports run asynchronously with permission, scope, purpose and download audit. Start with SQL projections and scheduled summaries; a warehouse is deferred until measured cross-country analytics harms operational workloads.

Report definitions must state audience, filter semantics, time zone, currency treatment, source entities, refresh cadence and permitted drill-down. Pastoral activity is reported as aggregate counts only, without case narratives. Give finance reports original currency totals; exchange-rate conversion needs explicit policy and source.

## Configurable engagement

`EngagementModelVersion` holds factor weights, windows, exclusions and explanation labels. Candidate factors: service/group attendance, ministry participation, volunteering, event attendance and curriculum progress. Do not include giving amount, pastoral/prayer data or inferred faithfulness. Calculate with visible source counts and a version; members/staff can challenge inaccurate source data. No automatic adverse membership decision may be based on score alone. Evaluate bias, missing data and offline attendance gaps before using the metric.

## Acceptance

Branch user cannot generate national member-level report; executive sees aggregate without confidential detail; report refresh timestamp shown; large export is auditable; engagement score explains each factor and version; correcting source attendance changes subsequent calculation without rewriting previous published report.
