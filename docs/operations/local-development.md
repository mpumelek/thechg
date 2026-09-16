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

The migration creates only the `organization` schema, `Churches` and `OrganizationalUnits`. It does not seed a real denomination, member or branch. Country creation is currently restricted by the internal application use case to South Africa (`ZA`, `ZAR`, `Africa/Johannesburg`). The schema remains country-neutral for a later approved Mozambique rollout.
