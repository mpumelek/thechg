# Isolated local development database

Status: development instructions only. Never point these commands at a shared or production database.

The Web project's `appsettings.Development.json` contains a **secret-free** SQL Server LocalDB example named `TheChgDevelopment`. LocalDB is available on Windows with the SQL Server Express LocalDB component. On macOS, Linux, CI or a different development SQL Server, set `ConnectionStrings__ChurchDatabase` in your own environment to an isolated database and keep credentials outside this repository. The app does not create or migrate databases on startup.

```powershell
dotnet tool restore
dotnet restore TheChg.slnx
dotnet build TheChg.slnx --no-restore
dotnet test TheChg.slnx --no-build
```

Before applying a migration, verify the connection string resolves to an empty, developer-owned database. Review the generated migration and SQL script:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet tool run dotnet-ef -- migrations script --idempotent --project src/TheChg.Infrastructure --startup-project src/TheChg.Web --context OrganizationDbContext
```

Only after that review, apply to the verified isolated target:

```powershell
dotnet tool run dotnet-ef -- database update --project src/TheChg.Infrastructure --startup-project src/TheChg.Web --context OrganizationDbContext
```

The organization migration creates the `organization` schema, `Churches` and `OrganizationalUnits`. The separate Identity context has its own `identity` schema and migrations, including a unique-email index, Church foreign key and invitation table. The Authorization context has an `authorization` schema and dated permission grants. To review or apply them, substitute `TheChgIdentityDbContext` and then `AuthorizationDbContext` for `OrganizationDbContext` in the commands above, reviewing each SQL script separately. Apply organization first, Identity second and Authorization third, only to a verified empty development database. These migrations seed no real denomination, member, account, branch or grant. The internal invitation service has no public issuance or redemption endpoint, and the Web login cannot be used until an account is deliberately provisioned and activated. Country creation and the protected branch-read use case are currently restricted to South Africa (`ZA`, `ZAR`, `Africa/Johannesburg`). The schema remains country-neutral for a later approved Mozambique rollout.
