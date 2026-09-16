using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TheChg.Infrastructure.Identity;
using TheChg.Domain.Organization;

namespace TheChg.Infrastructure.Tests.Identity;

public sealed class IdentityPersistenceTests
{
    [Fact]
    public void Account_requires_a_church_and_starts_inactive()
    {
        Assert.Throws<ArgumentException>(() => new ApplicationUser(Guid.Empty));

        var churchId = Guid.NewGuid();
        var account = new ApplicationUser(churchId);

        Assert.Equal(churchId, account.ChurchId);
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.False(account.IsActive);
    }

    [Fact]
    public async Task Account_round_trips_without_a_member_record()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
            .UseSqlite(connection).Options);
        await CreateIsolatedSchemaAsync(context);

        var churchId = Guid.NewGuid();
        context.Set<Church>().Add(Church.Create(churchId, "Synthetic Test Church"));
        await context.SaveChangesAsync();

        var account = new ApplicationUser(churchId)
        {
            UserName = "example@example.test",
            NormalizedUserName = "EXAMPLE@EXAMPLE.TEST",
            Email = "example@example.test",
            NormalizedEmail = "EXAMPLE@EXAMPLE.TEST"
        };
        context.Users.Add(account);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var loaded = await context.Users.AsNoTracking().SingleAsync(user => user.Id == account.Id);
        Assert.Equal(account.ChurchId, loaded.ChurchId);
        Assert.False(loaded.IsActive);
        Assert.DoesNotContain(context.Model.GetEntityTypes(), entity => entity.ClrType.Name == "Member");
    }

    [Fact]
    public async Task Duplicate_normalized_user_name_is_rejected_by_the_database()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
            .UseSqlite(connection).Options);
        await CreateIsolatedSchemaAsync(context);

        var churchId = Guid.NewGuid();
        context.Set<Church>().Add(Church.Create(churchId, "Synthetic Test Church"));
        await context.SaveChangesAsync();

        context.Users.Add(new ApplicationUser(churchId)
        {
            UserName = "first@example.test",
            NormalizedUserName = "DUPLICATE"
        });
        await context.SaveChangesAsync();

        context.Users.Add(new ApplicationUser(churchId)
        {
            UserName = "second@example.test",
            NormalizedUserName = "DUPLICATE"
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Duplicate_normalized_email_is_rejected_by_the_database()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
            .UseSqlite(connection).Options);
        await CreateIsolatedSchemaAsync(context);

        var churchId = Guid.NewGuid();
        context.Set<Church>().Add(Church.Create(churchId, "Synthetic Test Church"));
        await context.SaveChangesAsync();

        context.Users.Add(new ApplicationUser(churchId)
        {
            UserName = "first@example.test", NormalizedUserName = "FIRST",
            Email = "same@example.test", NormalizedEmail = "SAME@EXAMPLE.TEST"
        });
        await context.SaveChangesAsync();

        context.Users.Add(new ApplicationUser(churchId)
        {
            UserName = "second@example.test", NormalizedUserName = "SECOND",
            Email = "same@example.test", NormalizedEmail = "SAME@EXAMPLE.TEST"
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Account_without_church_is_rejected_by_the_database()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
            .UseSqlite(connection).Options);
        await CreateIsolatedSchemaAsync(context);

        context.Users.Add(new ApplicationUser { UserName = "unscoped@example.test" });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Account_with_nonexistent_church_is_rejected_by_the_database()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
            .UseSqlite(connection).Options);
        await CreateIsolatedSchemaAsync(context);

        context.Users.Add(new ApplicationUser(Guid.NewGuid()) { UserName = "orphan@example.test" });
        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public void Identity_tables_have_an_explicit_separate_schema()
    {
        using var context = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
            .UseSqlite("Data Source=:memory:").Options);

        var tables = context.Model.GetEntityTypes()
            .Select(entity => (entity.GetTableName(), entity.GetSchema()))
            .ToArray();
        Assert.All(tables.Where(table => table.Item1 != "Churches"),
            table => Assert.Equal("identity", table.Item2));
        Assert.Contains(tables, table => table.Item1 == "Churches" && table.Item2 == "organization");
        Assert.Contains(tables, table => table.Item1 == "Users");
        Assert.Contains(tables, table => table.Item1 == "Roles");
        Assert.Contains(tables, table => table.Item1 == "UserRoles");
    }

    [Fact]
    public void Registration_configures_sql_server_and_safe_identity_defaults()
    {
        var services = new ServiceCollection();
        Assert.Throws<ArgumentException>(() => services.AddIdentityPersistence(" "));

        services.AddIdentityPersistence("Server=(localdb)\\MSSQLLocalDB;Database=TheChg_IdentityRegistrationTest;Trusted_Connection=True");
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TheChgIdentityDbContext>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;

        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
        Assert.True(options.User.RequireUniqueEmail);
        Assert.True(options.SignIn.RequireConfirmedEmail);
        Assert.Equal(5, options.Lockout.MaxFailedAccessAttempts);
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>());
    }

    private static async Task CreateIsolatedSchemaAsync(TheChgIdentityDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        // Identity references this table but must not own or migrate it. SQLite ignores schemas.
        await context.Database.ExecuteSqlRawAsync(
            "CREATE TABLE Churches (Id TEXT NOT NULL PRIMARY KEY, Name TEXT NOT NULL);");
    }
}
