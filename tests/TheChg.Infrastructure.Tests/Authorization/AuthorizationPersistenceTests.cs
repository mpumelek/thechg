using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TheChg.Application.Authorization;
using TheChg.Domain.Organization;
using TheChg.Infrastructure.Authorization;
using TheChg.Infrastructure.Identity;
using TheChg.Infrastructure.Organization;

namespace TheChg.Infrastructure.Tests.Authorization;

public sealed class AuthorizationPersistenceTests
{
    [Fact]
    public async Task Account_reader_uses_persisted_inactive_state_and_church()
    {
        await using var fixture = await Fixture.CreateAsync();
        var church = Church.Create(Guid.NewGuid(), "Synthetic Church");
        fixture.Organization.Churches.Add(church);
        await fixture.Organization.SaveChangesAsync();
        var account = new ApplicationUser(church.Id) { UserName = "staff@example.test", NormalizedUserName = "STAFF@EXAMPLE.TEST" };
        fixture.Identity.Users.Add(account);
        await fixture.Identity.SaveChangesAsync();

        var loaded = await new EfAccessAccountReader(fixture.Identity).FindAsync(account.Id, default);
        Assert.Equal(new AccessAccount(account.Id, church.Id, false), loaded);
        Assert.Null(await new EfAccessAccountReader(fixture.Identity).FindAsync(Guid.NewGuid(), default));

        // Even a corrupted/provisioned active flag cannot bypass email verification.
        await fixture.Identity.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Users SET IsActive = {true} WHERE Id = {account.Id}");
        Assert.False((await new EfAccessAccountReader(fixture.Identity)
            .FindAsync(account.Id, default))!.IsActive);
    }

    [Fact]
    public async Task Circuit_descendants_are_explicit_and_sibling_branch_is_denied()
    {
        await using var fixture = await Fixture.CreateAsync();
        var hierarchy = await fixture.AddHierarchyAsync();
        var accountId = await fixture.AddAccountAsync(hierarchy.Church.Id);
        var now = DateTimeOffset.UtcNow;
        fixture.Authorization.PermissionGrants.Add(PermissionGrantRecord.Create(Guid.NewGuid(), accountId,
            hierarchy.Church.Id, "Organization.View", hierarchy.Circuit.Id, false,
            now.AddDays(-1), now.AddDays(1)));
        await fixture.Authorization.SaveChangesAsync();

        var evaluator = fixture.Evaluator(accountId, hierarchy.Church.Id);
        Assert.True(await evaluator.IsAllowedAsync(accountId, "Organization.View",
            new AuthorizationResource(hierarchy.Church.Id, hierarchy.Circuit.Id)));
        Assert.False(await evaluator.IsAllowedAsync(accountId, "Organization.View",
            new AuthorizationResource(hierarchy.Church.Id, hierarchy.Branch.Id)));

        fixture.Authorization.PermissionGrants.Add(PermissionGrantRecord.Create(Guid.NewGuid(), accountId,
            hierarchy.Church.Id, "Organization.View", hierarchy.Circuit.Id, true,
            now.AddHours(-12), now.AddDays(1)));
        await fixture.Authorization.SaveChangesAsync();
        Assert.True(await evaluator.IsAllowedAsync(accountId, "Organization.View",
            new AuthorizationResource(hierarchy.Church.Id, hierarchy.Branch.Id)));
        Assert.False(await evaluator.IsAllowedAsync(accountId, "Organization.View",
            new AuthorizationResource(hierarchy.Church.Id, hierarchy.SiblingBranch.Id)));
    }

    [Fact]
    public async Task Expired_future_and_revoked_grants_do_not_authorize()
    {
        await using var fixture = await Fixture.CreateAsync();
        var hierarchy = await fixture.AddHierarchyAsync();
        var accountId = await fixture.AddAccountAsync(hierarchy.Church.Id);
        var now = DateTimeOffset.UtcNow;
        var expired = PermissionGrantRecord.Create(Guid.NewGuid(), accountId, hierarchy.Church.Id,
            "Organization.View", hierarchy.Branch.Id, false, now.AddDays(-2), now.AddDays(-1));
        var future = PermissionGrantRecord.Create(Guid.NewGuid(), accountId, hierarchy.Church.Id,
            "Organization.View", hierarchy.Branch.Id, false, now.AddDays(1), now.AddDays(2));
        fixture.Authorization.PermissionGrants.AddRange(expired, future);
        await fixture.Authorization.SaveChangesAsync();
        var evaluator = fixture.Evaluator(accountId, hierarchy.Church.Id);
        var resource = new AuthorizationResource(hierarchy.Church.Id, hierarchy.Branch.Id);
        Assert.False(await evaluator.IsAllowedAsync(accountId, "Organization.View", resource));

        var active = PermissionGrantRecord.Create(Guid.NewGuid(), accountId, hierarchy.Church.Id,
            "Organization.View", hierarchy.Branch.Id, false, now.AddDays(-1), now.AddDays(1));
        fixture.Authorization.PermissionGrants.Add(active);
        await fixture.Authorization.SaveChangesAsync();
        Assert.True(await evaluator.IsAllowedAsync(accountId, "Organization.View", resource));

        active.Revoke(now);
        await fixture.Authorization.SaveChangesAsync();
        Assert.False(await evaluator.IsAllowedAsync(accountId, "Organization.View", resource));
    }

    [Fact]
    public async Task Cross_church_scope_and_resource_fail_closed()
    {
        await using var fixture = await Fixture.CreateAsync();
        var first = await fixture.AddHierarchyAsync();
        var other = await fixture.AddHierarchyAsync();
        var accountId = await fixture.AddAccountAsync(first.Church.Id);
        var now = DateTimeOffset.UtcNow;
        fixture.Authorization.PermissionGrants.Add(PermissionGrantRecord.Create(Guid.NewGuid(), accountId,
            first.Church.Id, "Organization.View", other.Branch.Id, true,
            now.AddDays(-1), now.AddDays(1)));
        await Assert.ThrowsAsync<DbUpdateException>(() => fixture.Authorization.SaveChangesAsync());
        fixture.Authorization.ChangeTracker.Clear();

        var reader = new EfPermissionGrantReader(fixture.Authorization, fixture.Organization);
        Assert.Empty(await reader.FindForAccountAsync(accountId, first.Church.Id, default));
        Assert.False(await fixture.Evaluator(accountId, first.Church.Id).IsAllowedAsync(accountId,
            "Organization.View", new AuthorizationResource(other.Church.Id, other.Branch.Id)));
    }

    [Fact]
    public async Task Ancestry_returns_target_to_country_and_rejects_missing_nodes()
    {
        await using var fixture = await Fixture.CreateAsync();
        var hierarchy = await fixture.AddHierarchyAsync();
        var reader = new EfOrganizationAncestryReader(fixture.Organization);
        var chain = await reader.FindInclusiveAncestryAsync(hierarchy.Branch.Id, default);

        Assert.Equal(new[] { hierarchy.Branch.Id, hierarchy.Circuit.Id, hierarchy.Country.Id },
            chain!.Select(node => node.Id));
        Assert.Null(await reader.FindInclusiveAncestryAsync(Guid.NewGuid(), default));
    }

    [Fact]
    public async Task Ancestry_rejects_a_same_church_but_wrong_parent_type()
    {
        await using var fixture = await Fixture.CreateAsync();
        var hierarchy = await fixture.AddHierarchyAsync();
        await fixture.Organization.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE OrganizationalUnits SET ParentId = {hierarchy.Country.Id} WHERE Id = {hierarchy.Branch.Id}");

        var reader = new EfOrganizationAncestryReader(fixture.Organization);
        Assert.Null(await reader.FindInclusiveAncestryAsync(hierarchy.Branch.Id, default));
    }

    [Fact]
    public async Task Duplicate_exact_assignment_is_rejected_by_the_database()
    {
        await using var fixture = await Fixture.CreateAsync();
        var hierarchy = await fixture.AddHierarchyAsync();
        var accountId = await fixture.AddAccountAsync(hierarchy.Church.Id);
        var now = DateTimeOffset.UtcNow;
        fixture.Authorization.PermissionGrants.Add(PermissionGrantRecord.Create(Guid.NewGuid(), accountId,
            hierarchy.Church.Id, "Organization.View", hierarchy.Branch.Id, false, now, null));
        await fixture.Authorization.SaveChangesAsync();

        fixture.Authorization.PermissionGrants.Add(PermissionGrantRecord.Create(Guid.NewGuid(), accountId,
            hierarchy.Church.Id, "Organization.View", hierarchy.Branch.Id, false, now, null));
        await Assert.ThrowsAsync<DbUpdateException>(() => fixture.Authorization.SaveChangesAsync());
    }

    [Fact]
    public void Invalid_grant_range_and_scope_shape_are_rejected()
    {
        var id = Guid.NewGuid();
        var now = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() => PermissionGrantRecord.Create(Guid.NewGuid(), id, id,
            "Organization.View", id, false, now, now));
        Assert.Throws<ArgumentException>(() => PermissionGrantRecord.Create(Guid.NewGuid(), id, id,
            "Organization.View", null, true, now, null));
    }

    private sealed class Fixture : IAsyncDisposable
    {
        private readonly SqliteConnection _identityConnection;
        private readonly SqliteConnection _organizationConnection;
        private readonly SqliteConnection _authorizationConnection;

        private Fixture(SqliteConnection identityConnection, SqliteConnection organizationConnection,
            SqliteConnection authorizationConnection)
        {
            _identityConnection = identityConnection;
            _organizationConnection = organizationConnection;
            _authorizationConnection = authorizationConnection;
            Identity = new TheChgIdentityDbContext(new DbContextOptionsBuilder<TheChgIdentityDbContext>()
                .UseSqlite(identityConnection).Options);
            Organization = new OrganizationDbContext(new DbContextOptionsBuilder<OrganizationDbContext>()
                .UseSqlite(organizationConnection).Options);
            Authorization = new AuthorizationDbContext(new DbContextOptionsBuilder<AuthorizationDbContext>()
                .UseSqlite(authorizationConnection).Options);
        }

        public TheChgIdentityDbContext Identity { get; }
        public OrganizationDbContext Organization { get; }
        public AuthorizationDbContext Authorization { get; }

        public static async Task<Fixture> CreateAsync()
        {
            var identity = new SqliteConnection("Data Source=:memory:");
            // Each context creates only its own tables in one synthetic database,
            // so SQLite enforces cross-schema references as well.
            var organization = identity;
            var authorization = identity;
            await identity.OpenAsync();
            var fixture = new Fixture(identity, organization, authorization);
            await fixture.Organization.Database.EnsureCreatedAsync();
            await fixture.Identity.Database.GetService<IRelationalDatabaseCreator>().CreateTablesAsync();
            await fixture.Authorization.Database.GetService<IRelationalDatabaseCreator>().CreateTablesAsync();
            return fixture;
        }

        public async Task<Guid> AddAccountAsync(Guid churchId)
        {
            var account = new ApplicationUser(churchId)
            {
                UserName = $"{Guid.NewGuid():N}@example.test",
                NormalizedUserName = $"{Guid.NewGuid():N}@EXAMPLE.TEST"
            };
            Identity.Users.Add(account);
            await Identity.SaveChangesAsync();
            return account.Id;
        }

        public async Task<(Church Church, OrganizationUnit Country, OrganizationUnit Circuit,
            OrganizationUnit Branch, OrganizationUnit SiblingBranch)> AddHierarchyAsync()
        {
            var church = Church.Create(Guid.NewGuid(), "Synthetic Church");
            var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "ZA", "South Africa",
                "Africa/Johannesburg", "ZAR");
            var circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-01", "Circuit 1");
            var branch = OrganizationUnit.CreateBranch(Guid.NewGuid(), circuit, "B-01", "Branch 1");
            var siblingCircuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-02", "Circuit 2");
            var siblingBranch = OrganizationUnit.CreateBranch(Guid.NewGuid(), siblingCircuit, "B-02", "Branch 2");
            Organization.Churches.Add(church);
            Organization.Units.AddRange(country, circuit, branch, siblingCircuit, siblingBranch);
            await Organization.SaveChangesAsync();
            return (church, country, circuit, branch, siblingBranch);
        }

        public ScopedAuthorizationEvaluator Evaluator(Guid accountId, Guid churchId) => new(
            new SyntheticActiveAccountReader(accountId, churchId),
            new EfPermissionGrantReader(Authorization, Organization),
            new EfOrganizationAncestryReader(Organization), TimeProvider.System);

        public async ValueTask DisposeAsync()
        {
            await Identity.DisposeAsync();
            await Organization.DisposeAsync();
            await Authorization.DisposeAsync();
            await _identityConnection.DisposeAsync();
            if (!ReferenceEquals(_organizationConnection, _identityConnection))
                await _organizationConnection.DisposeAsync();
            if (!ReferenceEquals(_authorizationConnection, _identityConnection))
                await _authorizationConnection.DisposeAsync();
        }
    }

    private sealed class SyntheticActiveAccountReader(Guid accountId, Guid churchId) : IAccessAccountReader
    {
        public Task<AccessAccount?> FindAsync(Guid id, CancellationToken cancellationToken) =>
            Task.FromResult<AccessAccount?>(id == accountId ? new AccessAccount(accountId, churchId, true) : null);
    }
}
