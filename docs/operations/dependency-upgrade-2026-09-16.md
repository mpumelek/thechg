# Dependency upgrade record — 16 September 2026

Status: **historical record of the previous repository**; not deployed and not applicable to the new solution's package versions. The previous MVC application was not imported.

## Scope and versions

| Component | Before | After |
|---|---|---|
| .NET target framework | 8.0 | 10.0 LTS |
| ASP.NET Core Identity, diagnostics, EF Core providers and EF tools | Mixed 8.0.x / 10.0.8 | 10.0.12 |
| Web code-generation design package | 8.0.23 | 10.0.2 |
| ClosedXML | 0.105.0 | 0.105.1 |
| QuestPDF | 2026.5.0 | 2026.9.0 |
| Bootstrap static assets | 5.1.0 | 5.3.8 |
| jQuery static assets | 3.6.0 | 3.7.1 |
| jQuery Validation static assets | 1.19.5 | 1.22.1 |
| jQuery Validation Unobtrusive static assets | 4.0.0 | 4.0.0 (unchanged) |
| Tailwind CSS dev dependency | 4.3.0 | 4.3.3 |

Versions are the latest stable releases verified on 16 September 2026 except for two deliberate compatibility pins:

- jQuery 4.0.0 is available, but `jquery-validation-unobtrusive` 4.0.0 declares a jQuery `^3.6.0` dependency. The app therefore uses the latest jQuery 3.x, 3.7.1, until the validation stack is verified on jQuery 4 or replaced.
- The code-generation package resolves vulnerable `NuGet.Packaging` and `NuGet.Protocol` 6.12.1 transitively. A private direct reference to `NuGet.Protocol` 6.12.5 keeps both on their patched 6.12 servicing line. NuGet 7.9.0 is newer but is not assumed compatible with the 10.0.2 scaffolder. Recheck this pin when upgrading the scaffolding toolchain.

Tailwind is present in `package.json` but is not wired into the current application's asset build. Updating it does not change the served CSS.

## Verification and rollout constraints

- `dotnet restore ChgManagementSystem.sln`: passed.
- `dotnet build ChgManagementSystem.sln --no-restore`: passed with pre-existing nullable-reference warnings; no errors.
- `dotnet list ChgManagementSystem/ChgManagementSystem.csproj package --vulnerable --include-transitive`: no vulnerable packages reported by configured sources.
- `dotnet test ChgManagementSystem.sln --no-restore --verbosity quiet`: exited successfully, but the solution has no automated test project, so this is **not** behavioral verification.
- `node --check` passed for the JavaScript files served by the layout and validation partials; `package.json` and `package-lock.json` agree on Tailwind 4.3.3.
- Razor references to checked-in CSS/JavaScript now append a content version so clients do not keep stale cached assets after deployment.
- The application was **not** started, and no database, migrations, payments, messages or hosting environment were touched.

Before deployment, add a representative automated regression suite; test Identity, giving, file handling and the upgraded front-end in an isolated environment; verify the target host supports the .NET 10 runtime; and rehearse a backup/rollback. The existing committed credentials and bootstrap password still require rotation/removal by an authorized owner.
