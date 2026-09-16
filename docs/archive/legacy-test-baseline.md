# Isolated legacy regression baseline

Status: **historical test baseline from the previous repository**. The commands below do not run in this new solution, and no legacy test code was imported. This is not acceptance of the target architecture or a production release.

The tests run against synthetic records in temporary in-memory SQLite. The HTTP test host replaces the app's SQL Server registration before startup, uses ephemeral data-protection keys, and uses a synthetic authentication scheme. It never uses the checked-in connection string or contacts a live database, payment gateway, or messaging provider.

Run from the repository root:

```powershell
dotnet restore tests/ChgManagementSystem.Tests/ChgManagementSystem.Tests.csproj --configfile tests/ChgManagementSystem.Tests/NuGet.Config
dotnet build ChgManagementSystem.sln --no-restore
dotnet test tests/ChgManagementSystem.Tests/ChgManagementSystem.Tests.csproj --no-restore
```

CI runs the build and tests independently of a GitHub-triggered secret scan and a NuGet audit. The new test project privately pins patched `NuGet.Packaging` 6.12.5 because the app's own private pin does not flow through a project reference; the strict low-severity audit passed locally. Existing committed credentials/default passwords still require owner-led rotation and remediation, so a history scan may report findings. A failing scan is a release blocker, not evidence that these regression tests failed. An organization-owned GitHub repository may need `GITLEAKS_LICENSE` configured as a repository or organization secret for the Gitleaks action. Push/PR scans must not be assumed to replace a separately evidenced full-history scan.

The full-history secret scan has **not run locally**. No Gitleaks, TruffleHog, git-secrets, Docker or Podman executable was available in this workspace. A portable Gitleaks Windows x64 release was considered but not executed because [its published checksum has been reported as mismatching](https://github.com/gitleaks/gitleaks/issues/2164). No remote history was fetched, no secret values were printed, and the GitHub CI job has not been run here. This is an unverified gate, not a passing result.

Coverage is deliberately narrow: current branch/member/tithe reads; anonymous and non-admin denial on selected management routes; published-only public news by list and direct ID; public gallery reads versus admin-only management; anti-forgery on selected privileged POSTs; GET-no-mutation and token-protected POST for branch-leader deletion and first-time website-settings creation. It does **not** prove cross-branch scope, member self-service isolation, giving immutability, upload safety/private media classification, webhook integrity, offline idempotency, SQL Server migrations, or complete route authorization. Those remain scheduled stories and require additional tests and review.
