# Organization and identity module specification

Status: Draft specification; **organization, inactive account persistence and authorization evaluation partially implemented**. Login, invitations, grant persistence, administration and audit are not implemented. Owners: Organization lead and security architect. Related: [permissions](../architecture/permissions-matrix.md), [ADR 0002](../adr/0002-church-id.md), [ADR 0003](../adr/0003-identity-and-scoped-authorization.md).

## Organization

One `Church` initially owns a tree of `OrganizationalUnit` records: country → circuit → branch, with optional intermediate types later. An active branch must have a country ancestor, country code, timezone and default currency. Prevent cycles, invalid parent/type combinations and orphaning units with operational records. Leadership assignments are effective-dated; former leaders retain history but lose authority when an assignment expires. Country/branch configuration is versioned and audited.

## Identity

`ApplicationUser` owns authentication, invitation, recovery and account status. `MemberUserLink` is verified and distinct from Church membership status. `RolePermission` defines reusable templates; `UserRoleAssignment` grants a role at a unit with effective dates and descendant policy. `RecordAccessGrant` is reserved for exceptional sensitive resources. An administrator may delegate only permissions and scope they are authorized to grant. Public login/registration endpoints cannot create official members.

## Workflow and security

Invite → verify account → establish credentials/MFA as required → activate scoped assignment. Disable account or revoke assignment independently; both must take effect promptly. Sessions are revocable. Changes to role, scope, MFA or account state create security audit records. Avoid embedding all permissions in long-lived tokens without revocation strategy.

## Acceptance

Create country/circuit/branch without schema change; deny invalid hierarchy/cycle; verify country/circuit descendant scopes; revoke access mid-session; prevent branch administrator granting national scope; prevent public self-registration from creating a member.
