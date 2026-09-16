# Canonical data model and data dictionary

Status: Draft. Owners: Database architect and module leads. This is a logical blueprint; create physical migrations only with the implementing slice. See the [table catalogue](church-management-platform-blueprint.md#38-initial-database-table-list).

## Common conventions

Owned records use `Id`, `ChurchId`, `CreatedAtUtc`, `CreatedBy`, `UpdatedAtUtc`, `UpdatedBy`, `RowVersion`. Include `BranchId` or relevant `OrgUnitId` where needed for direct authorization and historically correct reporting. Use UTC for instants and a named timezone on schedules. Money is decimal amount plus ISO currency. Prefer effective-dated relationships over overwriting branch/role/group history. Define indexes from real query plans, with unique constraints for immutable business identifiers.

| Concept | Logical fields and relationships | Classification / lifecycle |
|---|---|---|
| `Church` | `Id`, name, `status` | Internal; one initial row |
| `OrganizationalUnit` | `Id`, `ChurchId`, `ParentId`, type, country code, timezone, currency | Internal; tree cannot cycle |
| `Member` | `Id`, `ChurchId`, primary `BranchId`, person names, status, date joined | Personal and religious; never self-created |
| `MemberContact` | `MemberId`, kind, value, verified flags, preferred | Personal; protect and retain by policy |
| `FamilyRelationship` | two member/person IDs, relation type, effective dates | Personal; not automatic portal grant |
| `EmergencyContact` | `MemberId`, name, relationship, contact details | Sensitive; least privilege |
| `MembershipApplication` | `MemberId`, status, submitted snapshot/version, decision, reason | Governance record; immutable decisions |
| `MemberBranchHistory` | member, branch, effective from/to, transfer request | Historical; no overlaps for primary branch |
| `MemberJourney` | member, curriculum version, start/completion, status | Personal; version locked |
| `BuddyAssignment` | journey, buddy member, effective dates, status | Personal; capacity enforced |
| `CellGroupMembership` | group, member, effective dates | Personal; history retained |
| `AttendanceSession` | context type/id, branch, local schedule, UTC start | Operational |
| `AttendanceRecord` | session, member, method, recorded at/by | Personal; unique session/member |
| `PastoralCase` | subject, owner team, status, classification | Highly restricted, separate access grants |
| `Contribution` | member nullable, branch at transaction time, fund, amount, currency, source, status | Restricted finance; posted immutable |
| `PaymentProviderEvent` | provider, external event ID, verified flag, received time | Restricted; external ID unique |
| `Order` | purchaser, branch, currency, totals, status | Commercial; snapshot order lines |
| `Document` | owner type/id, object key, classification, checksum, retention | Metadata in SQL; binary private |

## Aggregate ownership and cross-module references

`membership.Member` owns membership status, not payments or attendance. Finance stores `MemberId` and branch/currency snapshot but does not update membership tables. Journey holds `MemberId` and queries permitted member details through a published contract. Pastoral records are not exposed through general search. Cross-module SQL foreign keys may be used selectively for stable IDs, but modules never rely on another module's EF navigation graph or write its tables.

## Integrity and retention

Use filtered uniqueness for one open membership application per member, `(DeviceId, ClientOperationId)` for offline submissions, `(Provider, ExternalEventId)` for webhooks, and unique check-in `(SessionId, MemberId)` where identified. Never cascade-delete finance, audit, pastoral or member history. Archive/status and approved retention processes replace ad hoc deletion. SQL temporal history may support selected profile changes but is not the audit ledger.

## Migration notes

Map current `Circuit`, `Branch`, `Member`, `TitheRecord` and `MonthlyOffering` data using source IDs and reconciliation counts. Do not infer that all existing members are approved or all amounts are posted without Church/finance sign-off. See [migration plan](../delivery/migration-and-cutover.md).
