# Permission and data-access matrix

Status: Draft. Owners: Security lead and Church leadership. This is a **permission design**, not approval of role assignments. Confirm titles and delegated authority before migration.

## Evaluation rule

Access requires an active account, permission grant, matching `ChurchId`, a target record within the grant's organizational scope, and any record-specific restriction. Country/circuit grants include descendants only when the assignment explicitly permits inheritance. The server derives a member's own record from `MemberUserLink`; a URL-supplied member ID never authorizes self-service. Pastoral and child records require extra policies even within a branch.

| Permission | Denomination | Country | Circuit | Branch | Member self | Extra restriction |
|---|---:|---:|---:|---:|---:|---|
| `Organization.View` | Yes | Yes | Yes | Yes | Public subset | Internal fields filtered |
| `Organization.Manage` | Yes | Configurable | Configurable | No | No | Cannot grant wider scope than own |
| `Member.Create` | Configurable | Configurable | Configurable | Yes | No | Branch administrator only by policy |
| `Staff.Register` | Configurable | Configurable | Configurable | Configurable | No | Capture pending branch request only; never grants or activates staff access |
| `Member.Approve` | Configurable | Configurable | Configurable | Configurable | No | Submitter cannot self-approve if policy requires separation; TBD |
| `Member.View` | Yes | Yes | Yes | Yes | Own | Protected fields separate |
| `Member.Edit` | Yes | Yes | Configurable | Configurable | Request only | Controlled fields require change request |
| `Member.Transfer` | Configurable | Configurable | Configurable | Configurable | Request only | Source/target approval policy TBD |
| `Member.ViewSensitive` | Narrow | Narrow | Narrow | Narrow | Own permitted | Reason and read audit |
| `Journey.Manage` | Yes | Yes | Yes | Yes | Own progress only | Published version immutable |
| `Buddy.Manage` | Yes | Yes | Yes | Yes | No | No confidential pastoral data |
| `Attendance.Capture` | Yes | Yes | Yes | Yes | Own check-in only | Valid session/device |
| `Finance.Capture` | Configurable | Configurable | Configurable | Configurable | No | Assigned fund/branch |
| `Finance.Post` | Configurable | Configurable | Configurable | Configurable | No | Server validation and audit |
| `Finance.View` | Yes | Yes | Yes | Yes | Own giving | Amount visibility policy |
| `Finance.Adjust` | Very narrow | Narrow | Narrow | Configurable | No | Reason and linked reversal |
| `PastoralCase.View` | No default | No default | No default | No default | Own submitted request only | Explicit case/team grant |
| `PastoralCase.Manage` | No default | No default | No default | No default | No | Case/team grant plus audit |
| `Store.ManageInventory` | Configurable | Configurable | Configurable | Configurable | No | Stock location scope |
| `Report.Executive` | Yes | Configurable | No | No | No | Aggregated, suppressed details |

`Yes` means a grant at that scope can be defined, not that every person with a title automatically receives it. Define role templates in configuration (`Branch Administrator`, `Amalunga`, etc.), then issue dated `UserRoleAssignment` records. Privileged access requires MFA; removal takes effect promptly and is audited.

## Acceptance tests

- Branch A administrator cannot read or update Branch B member, even by changing an ID.
- Circuit user sees only descendants of assigned circuit.
- Member sees only linked self record and own giving, not a spouse's by default.
- A normal branch administrator cannot open a pastoral case or confidential child document.
- User losing assignment cannot continue via cached UI/session authority.
- Every finance export and sensitive document read is logged.

Resource-based authorization is needed after loading a record; see [Microsoft guidance](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/resource-based?view=aspnetcore-10.0).
