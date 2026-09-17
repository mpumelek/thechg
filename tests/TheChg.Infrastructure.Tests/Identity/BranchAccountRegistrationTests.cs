using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using TheChg.Application.Registration;
using TheChg.Domain.Organization;
using TheChg.Infrastructure.Identity;
using TheChg.Infrastructure.Organization;

namespace TheChg.Infrastructure.Tests.Identity;

public sealed class BranchAccountRegistrationTests
{
    [Theory]
    [InlineData(BranchAccountKind.Member)]
    [InlineData(BranchAccountKind.Staff)]
    public async Task Branch_capture_creates_an_inactive_account_without_credentials_or_grants(
        BranchAccountKind kind)
    {
        await using var fixture = await Fixture.CreateAsync();

        var result = await fixture.Writer.CreatePendingAsync(fixture.RegistrarId, fixture.ChurchId,
            fixture.BranchId, kind, "new.person@example.test", default);

        fixture.Identity.ChangeTracker.Clear();
        var account = await fixture.Identity.Users.SingleAsync(user => user.Id == result.AccountId);
        var registration = await fixture.Identity.BranchAccountRegistrations.SingleAsync();
        Assert.False(account.IsActive);
        Assert.False(account.EmailConfirmed);
        Assert.Null(account.PasswordHash);
        Assert.Equal(fixture.BranchId, registration.BranchId);
        Assert.Equal(fixture.RegistrarId, registration.RegistrarId);
        Assert.Equal(kind, registration.Kind);
        Assert.Equal(result.Id, registration.Id);
    }

    [Fact]
    public async Task Duplicate_email_does_not_create_another_registration()
    {
        await using var fixture = await Fixture.CreateAsync();
        await fixture.Writer.CreatePendingAsync(fixture.RegistrarId, fixture.ChurchId,
            fixture.BranchId, BranchAccountKind.Member, "duplicate@example.test", default);

        await Assert.ThrowsAsync<DuplicateAccountRegistrationException>(() =>
            fixture.Writer.CreatePendingAsync(fixture.RegistrarId, fixture.ChurchId,
                fixture.BranchId, BranchAccountKind.Member, "duplicate@example.test", default));
        Assert.Equal(1, await fixture.Identity.BranchAccountRegistrations.CountAsync());
    }

    [Fact]
    public async Task Cross_church_branch_foreign_key_rejects_registration_and_rolls_back_account()
    {
        await using var fixture = await Fixture.CreateAsync();
        var otherChurch = Church.Create(Guid.NewGuid(), "Another synthetic Church");
        var otherCountry = OrganizationUnit.CreateCountry(Guid.NewGuid(), otherChurch, "ZA", "South Africa",
            "Africa/Johannesburg", "ZAR");
        var otherCircuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), otherCountry, "C-02", "Circuit");
        var otherBranch = OrganizationUnit.CreateBranch(Guid.NewGuid(), otherCircuit, "B-02", "Branch");
        fixture.Organization.Churches.Add(otherChurch);
        fixture.Organization.Units.AddRange(otherCountry, otherCircuit, otherBranch);
        await fixture.Organization.SaveChangesAsync();

        await Assert.ThrowsAsync<DbUpdateException>(() => fixture.Writer.CreatePendingAsync(
            fixture.RegistrarId, fixture.ChurchId, otherBranch.Id, BranchAccountKind.Member,
            "cross.church@example.test", default));

        fixture.Identity.ChangeTracker.Clear();
        Assert.Equal(1, await fixture.Identity.Users.CountAsync());
        Assert.Empty(await fixture.Identity.BranchAccountRegistrations.ToListAsync());
    }

    private sealed class Fixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _provider;

        private Fixture(SqliteConnection connection, ServiceProvider provider,
            TheChgIdentityDbContext identity, OrganizationDbContext organization,
            EfAccountRegistrationWriter writer, Guid registrarId, Guid churchId, Guid branchId)
        {
            _connection = connection;
            _provider = provider;
            Identity = identity;
            Organization = organization;
            Writer = writer;
            RegistrarId = registrarId;
            ChurchId = churchId;
            BranchId = branchId;
        }

        public TheChgIdentityDbContext Identity { get; }
        public OrganizationDbContext Organization { get; }
        public EfAccountRegistrationWriter Writer { get; }
        public Guid RegistrarId { get; }
        public Guid ChurchId { get; }
        public Guid BranchId { get; }

        public static async Task<Fixture> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(TimeProvider.System);
            services.AddDbContext<TheChgIdentityDbContext>(options => options.UseSqlite(connection));
            services.AddIdentityCore<ApplicationUser>(options => options.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<TheChgIdentityDbContext>();
            services.AddScoped<EfAccountRegistrationWriter>();
            var provider = services.BuildServiceProvider();
            var identity = provider.GetRequiredService<TheChgIdentityDbContext>();
            var organization = new OrganizationDbContext(
                new DbContextOptionsBuilder<OrganizationDbContext>().UseSqlite(connection).Options);
            await identity.Database.EnsureCreatedAsync();
            await organization.Database.GetService<IRelationalDatabaseCreator>().CreateTablesAsync();

            var church = Church.Create(Guid.NewGuid(), "Synthetic Church");
            var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "ZA", "South Africa",
                "Africa/Johannesburg", "ZAR");
            var circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-01", "Circuit");
            var branch = OrganizationUnit.CreateBranch(Guid.NewGuid(), circuit, "B-01", "Branch");
            organization.Churches.Add(church);
            organization.Units.AddRange(country, circuit, branch);
            await organization.SaveChangesAsync();

            var registrar = new ApplicationUser(church.Id)
            {
                UserName = "registrar@example.test", Email = "registrar@example.test"
            };
            var users = provider.GetRequiredService<UserManager<ApplicationUser>>();
            Assert.True((await users.CreateAsync(registrar)).Succeeded);

            return new Fixture(connection, provider, identity, organization,
                provider.GetRequiredService<EfAccountRegistrationWriter>(), registrar.Id, church.Id, branch.Id);
        }

        public async ValueTask DisposeAsync()
        {
            await Organization.DisposeAsync();
            await _provider.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
