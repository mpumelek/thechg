using TheChg.Application.Authorization;
using TheChg.Application.Organization;
using TheChg.Application.Registration;
using TheChg.Domain.Organization;

namespace TheChg.Application.Tests.Registration;

public sealed class BranchAccountRegistrationServiceTests
{
    [Theory]
    [InlineData(BranchAccountKind.Member, "Member.Create")]
    [InlineData(BranchAccountKind.Staff, "Staff.Register")]
    public async Task Authorized_branch_capture_creates_only_a_pending_request(
        BranchAccountKind kind, string permission)
    {
        var fixture = new Fixture(permission);

        var result = await fixture.Service.CaptureAsync(fixture.RegistrarId, fixture.Branch.Id,
            kind, "person@example.test");

        Assert.NotNull(result);
        Assert.Equal(kind, result.Kind);
        Assert.Equal(fixture.Branch.Id, result.BranchId);
        Assert.Equal(1, fixture.Writer.Calls);
        Assert.Equal(fixture.Church.Id, fixture.Writer.ChurchId);
        Assert.Equal(fixture.RegistrarId, fixture.Writer.RegistrarId);
    }

    [Fact]
    public async Task Member_capture_permission_does_not_register_staff()
    {
        var fixture = new Fixture("Member.Create");

        var result = await fixture.Service.CaptureAsync(fixture.RegistrarId, fixture.Branch.Id,
            BranchAccountKind.Staff, "person@example.test");

        Assert.Null(result);
        Assert.Equal(0, fixture.Writer.Calls);
    }

    [Fact]
    public async Task Sibling_branch_and_wrong_type_are_denied_without_writes()
    {
        var fixture = new Fixture("Member.Create");
        var sibling = OrganizationUnit.CreateBranch(Guid.NewGuid(), fixture.Circuit, "B-02", "Sibling");
        fixture.Repository.Add(sibling);

        Assert.Null(await fixture.Service.CaptureAsync(fixture.RegistrarId, sibling.Id,
            BranchAccountKind.Member, "person@example.test"));
        Assert.Null(await fixture.Service.CaptureAsync(fixture.RegistrarId, fixture.Circuit.Id,
            BranchAccountKind.Member, "person@example.test"));
        Assert.Null(await fixture.Service.CaptureAsync(fixture.RegistrarId, Guid.NewGuid(),
            BranchAccountKind.Member, "person@example.test"));
        Assert.Equal(0, fixture.Writer.Calls);
    }

    [Fact]
    public async Task Invalid_input_is_rejected_before_write()
    {
        var fixture = new Fixture("Member.Create");

        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.CaptureAsync(
            fixture.RegistrarId, fixture.Branch.Id, BranchAccountKind.Member, "not-an-email"));
        await Assert.ThrowsAsync<ArgumentException>(() => fixture.Service.CaptureAsync(
            fixture.RegistrarId, fixture.Branch.Id, (BranchAccountKind)99, "person@example.test"));
        Assert.Equal(0, fixture.Writer.Calls);
    }

    private sealed class Fixture
    {
        public Guid RegistrarId { get; } = Guid.NewGuid();
        public Church Church { get; } = Church.Create(Guid.NewGuid(), "Synthetic Church");
        public OrganizationUnit Circuit { get; }
        public OrganizationUnit Branch { get; }
        public FakeRepository Repository { get; } = new();
        public FakeWriter Writer { get; } = new();
        public BranchAccountRegistrationService Service { get; }

        public Fixture(string permission)
        {
            var country = OrganizationUnit.CreateCountry(Guid.NewGuid(), Church, "ZA", "South Africa",
                "Africa/Johannesburg", "ZAR");
            Circuit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, "C-01", "Circuit");
            Branch = OrganizationUnit.CreateBranch(Guid.NewGuid(), Circuit, "B-01", "Branch");
            Repository.Add(country);
            Repository.Add(Circuit);
            Repository.Add(Branch);

            var evaluator = new ScopedAuthorizationEvaluator(
                new AccountReader(RegistrarId, Church.Id),
                new GrantReader(RegistrarId, Church.Id, permission, Branch.Id),
                new AncestryReader(Church.Id, country.Id, Circuit.Id, Branch.Id), TimeProvider.System);
            Service = new BranchAccountRegistrationService(Repository, evaluator, Writer);
        }
    }

    private sealed class FakeRepository : IOrganizationRepository
    {
        private readonly Dictionary<Guid, OrganizationUnit> _units = [];
        public void Add(OrganizationUnit unit) => _units.Add(unit.Id, unit);
        public Task<OrganizationUnit?> FindUnitAsync(Guid id, CancellationToken ct) =>
            Task.FromResult(_units.GetValueOrDefault(id));
        public Task<Church?> FindChurchAsync(Guid id, CancellationToken ct) => Task.FromResult<Church?>(null);
        public Task<bool> CodeExistsAsync(Guid churchId, Guid? parentId, string code, CancellationToken ct) =>
            Task.FromResult(false);
        void IOrganizationRepository.Add(OrganizationUnit unit) => Add(unit);
        public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
    }

    private sealed class FakeWriter : IAccountRegistrationWriter
    {
        public int Calls { get; private set; }
        public Guid ChurchId { get; private set; }
        public Guid RegistrarId { get; private set; }
        public Task<PendingAccountRegistration> CreatePendingAsync(Guid registrarId, Guid churchId,
            Guid branchId, BranchAccountKind kind, string email, CancellationToken ct)
        {
            Calls++;
            ChurchId = churchId;
            RegistrarId = registrarId;
            return Task.FromResult(new PendingAccountRegistration(Guid.NewGuid(), Guid.NewGuid(), branchId, kind));
        }
    }

    private sealed class AccountReader(Guid id, Guid churchId) : IAccessAccountReader
    {
        public Task<AccessAccount?> FindAsync(Guid accountId, CancellationToken ct) =>
            Task.FromResult<AccessAccount?>(accountId == id ? new AccessAccount(id, churchId, true) : null);
    }

    private sealed class GrantReader(Guid id, Guid churchId, string permission, Guid scopeId) : IPermissionGrantReader
    {
        public Task<IReadOnlyCollection<PermissionGrant>> FindForAccountAsync(Guid accountId,
            Guid requestedChurchId, CancellationToken ct) => Task.FromResult<IReadOnlyCollection<PermissionGrant>>(
            accountId == id && requestedChurchId == churchId
                ? [new PermissionGrant(id, churchId, permission, scopeId, false,
                    DateTimeOffset.UtcNow.AddMinutes(-1), null)]
                : []);
    }

    private sealed class AncestryReader(Guid churchId, Guid countryId, Guid circuitId, Guid branchId)
        : IOrganizationAncestryReader
    {
        public Task<IReadOnlyList<OrganizationAncestor>?> FindInclusiveAncestryAsync(Guid id, CancellationToken ct)
        {
            IReadOnlyList<OrganizationAncestor>? chain = id == branchId
                ? [new(branchId, churchId), new(circuitId, churchId), new(countryId, churchId)]
                : null;
            return Task.FromResult(chain);
        }
    }
}
