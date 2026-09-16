using TheChg.Application.Organization;
using TheChg.Domain.Organization;

namespace TheChg.Application.Tests;

public sealed class OrganizationHierarchyServiceTests
{
    [Fact]
    public async Task Creates_only_South_Africa_as_the_current_country_scope()
    {
        var church = Church.Create(Guid.NewGuid(), "Test denomination");
        var repository = new FakeRepository(church);
        var service = new OrganizationHierarchyService(repository);

        var country = await service.CreateSouthAfricanCountryAsync(church.Id, "South Africa");

        Assert.Equal("ZA", country.Code);
        Assert.Equal("ZAR", country.CurrencyCode);
        Assert.Equal(1, repository.Saves);
    }

    [Fact]
    public async Task Duplicate_sibling_code_is_rejected_before_saving()
    {
        var church = Church.Create(Guid.NewGuid(), "Test denomination");
        var repository = new FakeRepository(church);
        var service = new OrganizationHierarchyService(repository);
        var country = await service.CreateSouthAfricanCountryAsync(church.Id, "South Africa");
        await service.CreateCircuitAsync(country.Id, "C-01", "Circuit 1");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateCircuitAsync(country.Id, "c-01", "Another circuit"));
        Assert.Equal(2, repository.Saves);
    }

    [Fact]
    public async Task Missing_parent_is_rejected()
    {
        var church = Church.Create(Guid.NewGuid(), "Test denomination");
        var service = new OrganizationHierarchyService(new FakeRepository(church));

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.CreateBranchAsync(Guid.NewGuid(), "B-01", "Branch"));
    }

    [Fact]
    public async Task Another_country_cannot_be_extended_in_this_release()
    {
        var church = Church.Create(Guid.NewGuid(), "Test denomination");
        var repository = new FakeRepository(church);
        var mozambique = OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "MZ", "Mozambique",
            "Africa/Maputo", "MZN");
        repository.Add(mozambique);
        var service = new OrganizationHierarchyService(repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateCircuitAsync(mozambique.Id, "C-01", "Circuit"));
    }

    private sealed class FakeRepository(Church church) : IOrganizationRepository
    {
        private readonly List<OrganizationUnit> _units = [];
        public int Saves { get; private set; }

        public Task<Church?> FindChurchAsync(Guid churchId, CancellationToken cancellationToken) =>
            Task.FromResult<Church?>(church.Id == churchId ? church : null);

        public Task<OrganizationUnit?> FindUnitAsync(Guid unitId, CancellationToken cancellationToken) =>
            Task.FromResult(_units.SingleOrDefault(unit => unit.Id == unitId));

        public Task<bool> CodeExistsAsync(Guid churchId, Guid? parentId, string code, CancellationToken cancellationToken) =>
            Task.FromResult(_units.Any(unit => unit.ChurchId == churchId && unit.ParentId == parentId && unit.Code == code));

        public void Add(OrganizationUnit unit) => _units.Add(unit);

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            Saves++;
            return Task.CompletedTask;
        }
    }
}
