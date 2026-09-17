using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheChg.Application.Authorization;
using TheChg.Application.Organization;
using TheChg.Application.Registration;
using TheChg.Domain.Organization;

namespace TheChg.Web.Tests.Security;

public sealed class BranchEndpointSecurityTests
{
    private static readonly DateTimeOffset Now = new(2026, 9, 16, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Anonymous_request_returns_401_without_branch_data()
    {
        using var fixture = new Fixture();
        var response = await fixture.GetBranchAsync(fixture.BranchA.Id, authenticated: false);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await AssertNoBranchDataAsync(response, fixture.BranchA);
    }

    [Fact]
    public async Task Authenticated_account_without_grant_returns_403()
    {
        using var fixture = new Fixture();
        var response = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await AssertNoBranchDataAsync(response, fixture.BranchA);
    }

    [Fact]
    public async Task Exact_branch_grant_allows_only_that_branch()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id));
        var allowed = await fixture.GetBranchAsync(fixture.BranchA.Id);
        var sibling = await fixture.GetBranchAsync(fixture.BranchB.Id);
        Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        var body = await allowed.Content.ReadAsStringAsync();
        Assert.Contains(fixture.BranchA.Name, body);
        Assert.DoesNotContain(fixture.BranchB.Name, body);
        using (var document = JsonDocument.Parse(body))
        {
            var fields = document.RootElement.EnumerateObject().Select(property => property.Name).ToHashSet();
            Assert.Equal(new[] { "id", "code", "name", "circuitId", "countryCode" }.ToHashSet(), fields);
        }
        Assert.Equal(HttpStatusCode.Forbidden, sibling.StatusCode);
        await AssertNoBranchDataAsync(sibling, fixture.BranchB);
    }

    [Fact]
    public async Task Different_circuit_grant_does_not_allow_branch_read()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.CircuitB.Id) with { IncludeDescendants = true });
        var response = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await AssertNoBranchDataAsync(response, fixture.BranchA);
    }

    [Fact]
    public async Task Circuit_grant_requires_explicit_descendant_flag()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.CircuitA.Id));
        var withoutInheritance = await fixture.GetBranchAsync(fixture.BranchA.Id);
        fixture.Grants[0] = fixture.Grants[0] with { IncludeDescendants = true };
        var withInheritance = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, withoutInheritance.StatusCode);
        Assert.Equal(HttpStatusCode.OK, withInheritance.StatusCode);
    }

    [Fact]
    public async Task Cross_church_account_is_denied_despite_a_matching_grant()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id));
        fixture.Account = fixture.Account with { ChurchId = Guid.NewGuid() };
        var response = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await AssertNoBranchDataAsync(response, fixture.BranchA);
    }

    [Fact]
    public async Task Circuit_id_cannot_be_read_through_branch_route()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.CircuitA.Id));
        var response = await fixture.GetBranchAsync(fixture.CircuitA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await AssertNoBranchDataAsync(response, fixture.CircuitA);
    }

    [Fact]
    public async Task Disabled_account_is_denied_with_an_active_grant()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id));
        fixture.Account = fixture.Account with { IsActive = false };
        var response = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        await AssertNoBranchDataAsync(response, fixture.BranchA);
    }

    [Fact]
    public async Task Expired_and_future_grants_are_denied()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id) with { EffectiveUntil = Now });
        var expired = await fixture.GetBranchAsync(fixture.BranchA.Id);
        fixture.Grants[0] = fixture.Grants[0] with
        {
            EffectiveFrom = Now.AddMinutes(1), EffectiveUntil = Now.AddMinutes(2)
        };
        var future = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.Forbidden, expired.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, future.StatusCode);
        await AssertNoBranchDataAsync(expired, fixture.BranchA);
        await AssertNoBranchDataAsync(future, fixture.BranchA);
    }

    [Fact]
    public async Task Revocation_denies_next_request_in_same_session()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id));
        var before = await fixture.GetBranchAsync(fixture.BranchA.Id);
        fixture.Grants.Clear();
        var after = await fixture.GetBranchAsync(fixture.BranchA.Id);
        Assert.Equal(HttpStatusCode.OK, before.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, after.StatusCode);
        await AssertNoBranchDataAsync(after, fixture.BranchA);
    }

    [Fact]
    public async Task Unknown_branch_does_not_reveal_more_than_known_unauthorized_branch()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id));
        var sibling = await fixture.GetBranchAsync(fixture.BranchB.Id);
        var unknown = await fixture.GetBranchAsync(Guid.NewGuid());
        Assert.Equal(HttpStatusCode.Forbidden, sibling.StatusCode);
        Assert.Equal(sibling.StatusCode, unknown.StatusCode);
        Assert.Equal(await sibling.Content.ReadAsStringAsync(), await unknown.Content.ReadAsStringAsync());
        await AssertNoBranchDataAsync(sibling, fixture.BranchB);
    }

    [Fact]
    public async Task Branch_account_capture_requires_a_matching_kind_and_branch_grant()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id) with { Permission = "Member.Create" });

        var allowed = await fixture.PostRegistrationAsync(fixture.BranchA.Id, "Member");
        var staff = await fixture.PostRegistrationAsync(fixture.BranchA.Id, "Staff");
        var sibling = await fixture.PostRegistrationAsync(fixture.BranchB.Id, "Member");

        Assert.Equal(HttpStatusCode.Created, allowed.StatusCode);
        Assert.Contains("PendingReview", await allowed.Content.ReadAsStringAsync());
        Assert.Equal(HttpStatusCode.Forbidden, staff.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, sibling.StatusCode);
        Assert.Equal(1, fixture.RegistrationWrites);
    }

    [Fact]
    public async Task Branch_account_capture_without_antiforgery_token_is_rejected()
    {
        using var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(fixture.BranchA.Id) with { Permission = "Member.Create" });

        var response = await fixture.PostRegistrationAsync(fixture.BranchA.Id, "Member", includeToken: false);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, fixture.RegistrationWrites);
    }

    private static async Task AssertNoBranchDataAsync(HttpResponseMessage response, OrganizationUnit branch)
    {
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain(branch.Name, body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(branch.Code, body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(branch.Id.ToString(), body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(branch.ChurchId.ToString(), body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class Fixture : IDisposable, IOrganizationRepository, IAccessAccountReader,
        IPermissionGrantReader, IOrganizationAncestryReader, IAccountRegistrationWriter
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Church _church = Church.Create(Guid.NewGuid(), "Synthetic Church");
        private readonly OrganizationUnit _country;
        private readonly Dictionary<Guid, OrganizationUnit> _units;
        private readonly Guid _accountId = Guid.NewGuid();

        public Fixture()
        {
            _country = OrganizationUnit.CreateCountry(Guid.NewGuid(), _church, "ZA", "South Africa", "Africa/Johannesburg", "ZAR");
            CircuitA = OrganizationUnit.CreateCircuit(Guid.NewGuid(), _country, "CIR-A", "Circuit A");
            CircuitB = OrganizationUnit.CreateCircuit(Guid.NewGuid(), _country, "CIR-B", "Circuit B");
            BranchA = OrganizationUnit.CreateBranch(Guid.NewGuid(), CircuitA, "BR-A", "Allowed Branch");
            BranchB = OrganizationUnit.CreateBranch(Guid.NewGuid(), CircuitA, "BR-B", "Sibling Branch");
            _units = new[] { _country, CircuitA, CircuitB, BranchA, BranchB }.ToDictionary(unit => unit.Id);
            Account = new AccessAccount(_accountId, _church.Id, true);
            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<IOrganizationRepository>();
                    services.RemoveAll<IAccessAccountReader>();
                    services.RemoveAll<IPermissionGrantReader>();
                    services.RemoveAll<IOrganizationAncestryReader>();
                    services.RemoveAll<IAccountRegistrationWriter>();
                    services.RemoveAll<TimeProvider>();
                    services.AddSingleton<IOrganizationRepository>(this);
                    services.AddSingleton<IAccessAccountReader>(this);
                    services.AddSingleton<IPermissionGrantReader>(this);
                    services.AddSingleton<IOrganizationAncestryReader>(this);
                    services.AddSingleton<IAccountRegistrationWriter>(this);
                    services.AddSingleton<TimeProvider>(new FixedClock());
                    services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = SyntheticAuthenticationHandler.SchemeName;
                            options.DefaultChallengeScheme = SyntheticAuthenticationHandler.SchemeName;
                            options.DefaultForbidScheme = SyntheticAuthenticationHandler.SchemeName;
                        })
                        .AddScheme<AuthenticationSchemeOptions, SyntheticAuthenticationHandler>(
                            SyntheticAuthenticationHandler.SchemeName, _ => { });
                }));
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("https://localhost")
            });
        }

        public OrganizationUnit CircuitA { get; }
        public OrganizationUnit CircuitB { get; }
        public OrganizationUnit BranchA { get; }
        public OrganizationUnit BranchB { get; }
        public AccessAccount Account { get; set; }
        public List<PermissionGrant> Grants { get; } = [];
        public int RegistrationWrites { get; private set; }

        public PermissionGrant Grant(Guid? scopeId) =>
            new(_accountId, _church.Id, "Organization.View", scopeId, false, Now.AddDays(-1), null);

        public Task<HttpResponseMessage> GetBranchAsync(Guid branchId, bool authenticated = true)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/organization/branches/{branchId}");
            if (authenticated)
                request.Headers.Add(SyntheticAuthenticationHandler.AccountHeader, _accountId.ToString());
            return _client.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PostRegistrationAsync(Guid branchId, string kind,
            bool includeToken = true)
        {
            string? token = null;
            if (includeToken)
            {
                using var tokenRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/security/antiforgery");
                tokenRequest.Headers.Add(SyntheticAuthenticationHandler.AccountHeader, _accountId.ToString());
                using var tokenResponse = await _client.SendAsync(tokenRequest);
                tokenResponse.EnsureSuccessStatusCode();
                using var body = JsonDocument.Parse(await tokenResponse.Content.ReadAsStringAsync());
                token = body.RootElement.GetProperty("requestToken").GetString();
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/registrations/accounts");
            request.Headers.Add(SyntheticAuthenticationHandler.AccountHeader, _accountId.ToString());
            if (token is not null)
                request.Headers.Add("X-CSRF-TOKEN", token);
            request.Content = JsonContent.Create(new
            {
                branchId, kind, email = "synthetic.person@example.test"
            });
            return await _client.SendAsync(request);
        }

        public Task<PendingAccountRegistration> CreatePendingAsync(Guid registrarId, Guid churchId,
            Guid branchId, BranchAccountKind kind, string email, CancellationToken cancellationToken)
        {
            RegistrationWrites++;
            return Task.FromResult(new PendingAccountRegistration(Guid.NewGuid(), Guid.NewGuid(), branchId, kind));
        }

        public Task<AccessAccount?> FindAsync(Guid accountId, CancellationToken cancellationToken) =>
            Task.FromResult<AccessAccount?>(accountId == _accountId ? Account : null);

        public Task<IReadOnlyCollection<PermissionGrant>> FindForAccountAsync(
            Guid accountId, Guid churchId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<PermissionGrant>>(Grants.ToArray());

        public Task<IReadOnlyList<OrganizationAncestor>?> FindInclusiveAncestryAsync(
            Guid organizationUnitId, CancellationToken cancellationToken)
        {
            if (!_units.TryGetValue(organizationUnitId, out var current))
                return Task.FromResult<IReadOnlyList<OrganizationAncestor>?>(null);
            var ancestry = new List<OrganizationAncestor>();
            do
            {
                ancestry.Add(new OrganizationAncestor(current.Id, current.ChurchId));
            } while (current.ParentId is Guid parentId && _units.TryGetValue(parentId, out current));
            return Task.FromResult<IReadOnlyList<OrganizationAncestor>?>(ancestry);
        }

        public Task<Church?> FindChurchAsync(Guid churchId, CancellationToken cancellationToken) =>
            Task.FromResult<Church?>(churchId == _church.Id ? _church : null);

        public Task<OrganizationUnit?> FindUnitAsync(Guid unitId, CancellationToken cancellationToken) =>
            Task.FromResult(_units.GetValueOrDefault(unitId));

        public Task<bool> CodeExistsAsync(Guid churchId, Guid? parentId, string code,
            CancellationToken cancellationToken) => Task.FromResult(false);

        public void Add(OrganizationUnit unit) => throw new NotSupportedException("Read-only test fixture.");
        public Task SaveChangesAsync(CancellationToken cancellationToken) =>
            throw new NotSupportedException("Read-only test fixture.");

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class SyntheticAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SchemeName = "SyntheticStaff";
        public const string AccountHeader = "X-Test-Account-Id";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(AccountHeader, out var values) ||
                !Guid.TryParse(values.ToString(), out var accountId))
                return Task.FromResult(AuthenticateResult.NoResult());
            var principal = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, accountId.ToString())], SchemeName));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
        }
    }
}
