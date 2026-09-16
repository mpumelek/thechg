using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheChg.Domain.Organization;
using TheChg.Infrastructure.Identity;

namespace TheChg.Infrastructure.Tests.Identity;

public sealed class AccountInvitationServiceTests
{
    private const string Destination = "synthetic.staff@example.test";
    private const string StrongPassword = "Synthetic-Only-2026!";

    [Fact]
    public async Task Redemption_is_destination_bound_single_use_and_activates_account()
    {
        await using var fixture = await Fixture.CreateAsync();
        var token = await fixture.Invitations.IssueAsync(fixture.AccountId, Destination, TimeSpan.FromHours(2));
        var invitation = await fixture.Context.Invitations.SingleAsync();

        Assert.NotEqual(token, invitation.TokenHash);
        Assert.Equal(64, invitation.TokenHash.Length);
        Assert.False(await fixture.Invitations.RedeemAsync(token, "wrong@example.test", StrongPassword));
        Assert.False(await fixture.Context.Users.Where(user => user.Id == fixture.AccountId)
            .Select(user => user.IsActive).SingleAsync());

        Assert.True(await fixture.Invitations.RedeemAsync(token, Destination, StrongPassword));
        Assert.False(await fixture.Invitations.RedeemAsync(token, Destination, StrongPassword));

        fixture.Context.ChangeTracker.Clear();
        var account = await fixture.Context.Users.SingleAsync(user => user.Id == fixture.AccountId);
        Assert.True(account.IsActive);
        Assert.True(account.EmailConfirmed);
        Assert.True(await fixture.UserManager.CheckPasswordAsync(account, StrongPassword));
        Assert.NotNull((await fixture.Context.Invitations.SingleAsync()).ConsumedAt);
    }

    [Fact]
    public async Task Expired_invitation_cannot_be_redeemed()
    {
        await using var fixture = await Fixture.CreateAsync();
        var token = await fixture.Invitations.IssueAsync(fixture.AccountId, Destination, TimeSpan.FromMinutes(1));
        fixture.Clock.Advance(TimeSpan.FromMinutes(2));

        Assert.False(await fixture.Invitations.RedeemAsync(token, Destination, StrongPassword));
        Assert.False((await fixture.Context.Users.SingleAsync()).IsActive);
        Assert.Null((await fixture.Context.Invitations.SingleAsync()).ConsumedAt);
    }

    [Fact]
    public async Task Weak_password_does_not_consume_invitation_or_activate_account()
    {
        await using var fixture = await Fixture.CreateAsync();
        var token = await fixture.Invitations.IssueAsync(fixture.AccountId, Destination, TimeSpan.FromHours(1));

        Assert.False(await fixture.Invitations.RedeemAsync(token, Destination, "weak"));
        Assert.Null((await fixture.Context.Invitations.SingleAsync()).ConsumedAt);
        Assert.False((await fixture.Context.Users.SingleAsync()).IsActive);
        Assert.True(await fixture.Invitations.RedeemAsync(token, Destination, StrongPassword));
    }

    [Fact]
    public async Task Issuance_rejects_another_destination_and_invalid_lifetime()
    {
        await using var fixture = await Fixture.CreateAsync();
        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Invitations.IssueAsync(
            fixture.AccountId, "other@example.test", TimeSpan.FromHours(1)));
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Invitations.IssueAsync(
            fixture.AccountId, Destination, TimeSpan.FromDays(8)));
        Assert.Empty(fixture.Context.Invitations);
    }

    [Fact]
    public async Task Issuance_rejects_account_with_existing_credentials()
    {
        await using var fixture = await Fixture.CreateAsync();
        var account = await fixture.Context.Users.SingleAsync();
        Assert.True((await fixture.UserManager.AddPasswordAsync(account, StrongPassword)).Succeeded);

        await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.Invitations.IssueAsync(
            fixture.AccountId, Destination, TimeSpan.FromHours(1)));
        Assert.Empty(fixture.Context.Invitations);
    }

    private sealed class Fixture : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly ServiceProvider _provider;

        private Fixture(SqliteConnection connection, ServiceProvider provider, TheChgIdentityDbContext context,
            UserManager<ApplicationUser> userManager, AccountInvitationService invitations, TestClock clock,
            Guid accountId)
        {
            _connection = connection;
            _provider = provider;
            Context = context;
            UserManager = userManager;
            Invitations = invitations;
            Clock = clock;
            AccountId = accountId;
        }

        public TheChgIdentityDbContext Context { get; }
        public UserManager<ApplicationUser> UserManager { get; }
        public AccountInvitationService Invitations { get; }
        public TestClock Clock { get; }
        public Guid AccountId { get; }

        public static async Task<Fixture> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var clock = new TestClock();
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<TheChgIdentityDbContext>(options => options.UseSqlite(connection));
            services.AddSingleton<TimeProvider>(clock);
            services.AddScoped<AccountInvitationService>();
            services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 12;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                })
                .AddEntityFrameworkStores<TheChgIdentityDbContext>();
            var provider = services.BuildServiceProvider();
            var context = provider.GetRequiredService<TheChgIdentityDbContext>();
            await context.Database.EnsureCreatedAsync();
            await context.Database.ExecuteSqlRawAsync(
                "CREATE TABLE Churches (Id TEXT NOT NULL PRIMARY KEY, Name TEXT NOT NULL);");
            var churchId = Guid.NewGuid();
            context.Set<Church>().Add(Church.Create(churchId, "Synthetic Test Church"));
            await context.SaveChangesAsync();

            var manager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var account = new ApplicationUser(churchId) { UserName = Destination, Email = Destination };
            Assert.True((await manager.CreateAsync(account)).Succeeded);
            return new Fixture(connection, provider, context, manager,
                provider.GetRequiredService<AccountInvitationService>(), clock, account.Id);
        }

        public async ValueTask DisposeAsync()
        {
            await _provider.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }

    private sealed class TestClock : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 9, 16, 10, 0, 0, TimeSpan.Zero);
        public override DateTimeOffset GetUtcNow() => _now;
        public void Advance(TimeSpan duration) => _now = _now.Add(duration);
    }
}
