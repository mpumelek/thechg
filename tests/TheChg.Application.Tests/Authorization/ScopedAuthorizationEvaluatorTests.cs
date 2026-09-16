using TheChg.Application.Authorization;

namespace TheChg.Application.Tests.Authorization;

public sealed class ScopedAuthorizationEvaluatorTests
{
    private static readonly Guid AccountId = Guid.NewGuid();
    private static readonly Guid ChurchId = Guid.NewGuid();
    private static readonly Guid CountryId = Guid.NewGuid();
    private static readonly Guid CircuitId = Guid.NewGuid();
    private static readonly Guid BranchId = Guid.NewGuid();
    private static readonly DateTimeOffset Now = new(2026, 9, 16, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Exact_branch_grant_allows_matching_permission()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.View"));

        Assert.True(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Inactive_or_missing_account_is_denied()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.View"));
        fixture.Account = new AccessAccount(AccountId, ChurchId, false);

        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));

        fixture.Account = null;
        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Exact_permission_and_church_are_required_even_if_reader_returns_bad_grants()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.Edit"));
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.View") with { ChurchId = Guid.NewGuid() });
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.View") with { AccountId = Guid.NewGuid() });

        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Other_branch_is_denied()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(Guid.NewGuid(), "Member.View"));

        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Ancestor_grant_needs_explicit_descendant_permission()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(CircuitId, "Organization.View"));

        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Organization.View", fixture.BranchResource));

        fixture.Grants[0] = fixture.Grants[0] with { IncludeDescendants = true };
        Assert.True(await fixture.Evaluator.IsAllowedAsync(AccountId, "Organization.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Child_grant_never_authorizes_ancestor()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "Organization.View") with { IncludeDescendants = true });
        var circuit = new AuthorizationResource(ChurchId, CircuitId);
        fixture.Ancestors = [new(CircuitId, ChurchId), new(CountryId, ChurchId)];

        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Organization.View", circuit));
    }

    [Fact]
    public async Task Grant_effective_dates_are_checked_on_each_evaluation()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.View") with { EffectiveFrom = Now.AddMinutes(1) });
        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));

        fixture.Grants[0] = fixture.Grants[0] with { EffectiveFrom = Now.AddMinutes(-1), EffectiveUntil = Now };
        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));

        fixture.Grants[0] = fixture.Grants[0] with { EffectiveUntil = Now.AddMinutes(1) };
        Assert.True(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Revocation_is_observed_without_recreating_evaluator()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "Member.View"));
        Assert.True(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));

        fixture.Grants.Clear();
        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Missing_or_cross_church_ancestry_is_denied()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(null, "Member.View"));
        fixture.Ancestors = null;
        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));

        fixture.Ancestors = [new(BranchId, Guid.NewGuid()), new(CircuitId, ChurchId)];
        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "Member.View", fixture.BranchResource));
    }

    [Fact]
    public async Task Sensitive_records_are_denied_without_additional_policy()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "PastoralCase.View"));
        var sensitive = fixture.BranchResource with { RecordId = Guid.NewGuid(), IsSensitive = true };

        Assert.False(await fixture.Evaluator.IsAllowedAsync(AccountId, "PastoralCase.View", sensitive));
    }

    [Fact]
    public async Task Sensitive_records_require_explicit_positive_record_policy()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(BranchId, "PastoralCase.View"));
        var sensitive = fixture.BranchResource with { RecordId = Guid.NewGuid(), IsSensitive = true };
        var policy = new TestSensitivePolicy();
        var evaluator = fixture.WithPolicy(policy);

        Assert.False(await evaluator.IsAllowedAsync(AccountId, "PastoralCase.View", sensitive));
        policy.Allow = true;
        Assert.True(await evaluator.IsAllowedAsync(AccountId, "PastoralCase.View", sensitive));
    }

    [Fact]
    public async Task Church_wide_grant_only_applies_with_matching_church_id()
    {
        var fixture = new Fixture();
        fixture.Grants.Add(fixture.Grant(null, "Organization.View"));
        Assert.True(await fixture.Evaluator.IsAllowedAsync(AccountId, "Organization.View", fixture.BranchResource));

        Assert.False(await fixture.Evaluator.IsAllowedAsync(
            AccountId, "Organization.View", new AuthorizationResource(Guid.NewGuid(), null)));
    }

    [Fact]
    public async Task Account_church_must_match_even_if_a_bad_grant_reader_returns_a_grant()
    {
        var fixture = new Fixture();
        fixture.Account = new AccessAccount(AccountId, Guid.NewGuid(), true);
        fixture.Grants.Add(fixture.Grant(null, "Organization.View"));

        Assert.False(await fixture.Evaluator.IsAllowedAsync(
            AccountId, "Organization.View", new AuthorizationResource(ChurchId, null)));
    }

    private sealed class Fixture : IAccessAccountReader, IPermissionGrantReader, IOrganizationAncestryReader
    {
        public AccessAccount? Account { get; set; } = new(AccountId, ChurchId, true);
        public List<PermissionGrant> Grants { get; } = [];
        public IReadOnlyList<OrganizationAncestor>? Ancestors { get; set; } =
            [new(BranchId, ChurchId), new(CircuitId, ChurchId), new(CountryId, ChurchId)];
        public AuthorizationResource BranchResource => new(ChurchId, BranchId);
        public ScopedAuthorizationEvaluator Evaluator => new(this, this, this, new FixedClock());

        public PermissionGrant Grant(Guid? scopeUnitId, string permission) =>
            new(AccountId, ChurchId, permission, scopeUnitId, false, Now.AddDays(-1), null);

        public ScopedAuthorizationEvaluator WithPolicy(ISensitiveRecordAccessPolicy policy) =>
            new(this, this, this, new FixedClock(), policy);

        public Task<AccessAccount?> FindAsync(Guid accountId, CancellationToken cancellationToken) =>
            Task.FromResult(Account);

        public Task<IReadOnlyCollection<PermissionGrant>> FindForAccountAsync(
            Guid accountId, Guid churchId, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<PermissionGrant>>(Grants);

        public Task<IReadOnlyList<OrganizationAncestor>?> FindInclusiveAncestryAsync(
            Guid organizationUnitId, CancellationToken cancellationToken) =>
            Task.FromResult(Ancestors);
    }

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => Now;
    }

    private sealed class TestSensitivePolicy : ISensitiveRecordAccessPolicy
    {
        public bool Allow { get; set; }
        public Task<bool> IsAllowedAsync(Guid accountId, string permission, AuthorizationResource resource,
            CancellationToken cancellationToken) => Task.FromResult(Allow);
    }
}
