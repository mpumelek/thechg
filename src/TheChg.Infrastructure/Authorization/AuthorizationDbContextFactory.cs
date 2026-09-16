using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TheChg.Infrastructure.Authorization;

/// <summary>Design-time only; migration generation never contacts this placeholder database.</summary>
public sealed class AuthorizationDbContextFactory : IDesignTimeDbContextFactory<AuthorizationDbContext>
{
    public AuthorizationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuthorizationDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TheChg_DesignTimeOnly;Trusted_Connection=True")
            .Options;
        return new AuthorizationDbContext(options);
    }
}
