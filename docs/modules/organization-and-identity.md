# Organization and identity module specification

Status: Draft specification; **organization, staff login, inactive account persistence, branch account-request capture and scoped grant evaluation partially implemented**. Account approval/activation, official membership registration, administration and full audit are not implemented. Owners: Organization lead and security architect. Related: [permissions](../architecture/permissions-matrix.md), [ADR 0002](../adr/0002-church-id.md), [ADR 0003](../adr/0003-identity-and-scoped-authorization.md), [ADR 0005](../adr/0005-branch-led-registration.md).

## Organization

One `Church` initially owns a tree of `OrganizationalUnit` records: country → circuit → branch, with optional intermediate types later. An active branch must have a country ancestor, country code, timezone and default currency. Prevent cycles, invalid parent/type combinations and orphaning units with operational records. Leadership assignments are effective-dated; former leaders retain history but lose authority when an assignment expires. Country/branch configuration is versioned and audited.

## Identity

`ApplicationUser` owns authentication, recovery and account status. All staff and member account requests are captured in person at branches by scoped registrars and remain inactive pending review. `MemberUserLink` is verified and distinct from Church membership status. `RolePermission` defines reusable templates; `UserRoleAssignment` grants a role at a unit with effective dates and descendant policy. `RecordAccessGrant` is reserved for exceptional sensitive resources. An administrator may delegate only permissions and scope they are authorized to grant. Public login cannot create official members; public self-registration is absent.

## Workflow and security

Branch registrar captures request → verify person and duplicates in person → independent approval as policy requires → person privately establishes credentials/MFA at branch → activate account and, for staff, separately grant scoped authority. No invitation is issued. Disable account or revoke assignment independently; both must take effect promptly. Sessions are revocable. Changes to role, scope, MFA or account state create security audit records. Avoid embedding all permissions in long-lived tokens without revocation strategy.

## Acceptance

Create country/circuit/branch without schema change; deny invalid hierarchy/cycle; verify country/circuit descendant scopes; revoke access mid-session; prevent branch administrator granting national scope; prevent public self-registration from creating a member.
