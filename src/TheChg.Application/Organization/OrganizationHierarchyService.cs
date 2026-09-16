using TheChg.Domain.Organization;

namespace TheChg.Application.Organization;

/// <summary>Internal use cases. No public write endpoint is exposed before scoped authorization exists.</summary>
public sealed class OrganizationHierarchyService(IOrganizationRepository repository)
{
    public async Task<OrganizationUnit> CreateSouthAfricanCountryAsync(Guid churchId, string name,
        CancellationToken cancellationToken = default)
    {
        var church = await repository.FindChurchAsync(churchId, cancellationToken)
            ?? throw new KeyNotFoundException("Church not found.");
        var unit = OrganizationUnit.CreateCountry(Guid.NewGuid(), church, "ZA", name,
            "Africa/Johannesburg", "ZAR");
        return await AddUniqueAsync(unit, cancellationToken);
    }

    public async Task<OrganizationUnit> CreateCircuitAsync(Guid countryId, string code, string name,
        CancellationToken cancellationToken = default)
    {
        var country = await repository.FindUnitAsync(countryId, cancellationToken)
            ?? throw new KeyNotFoundException("Country not found.");
        RequireSouthAfricanScope(country);
        var unit = OrganizationUnit.CreateCircuit(Guid.NewGuid(), country, code, name);
        return await AddUniqueAsync(unit, cancellationToken);
    }

    public async Task<OrganizationUnit> CreateBranchAsync(Guid circuitId, string code, string name,
        CancellationToken cancellationToken = default)
    {
        var circuit = await repository.FindUnitAsync(circuitId, cancellationToken)
            ?? throw new KeyNotFoundException("Circuit not found.");
        RequireSouthAfricanScope(circuit);
        var unit = OrganizationUnit.CreateBranch(Guid.NewGuid(), circuit, code, name);
        return await AddUniqueAsync(unit, cancellationToken);
    }

    private async Task<OrganizationUnit> AddUniqueAsync(OrganizationUnit unit, CancellationToken cancellationToken)
    {
        if (await repository.CodeExistsAsync(unit.ChurchId, unit.ParentId, unit.Code, cancellationToken))
            throw new InvalidOperationException("An organization unit with this code already exists under the parent.");
        repository.Add(unit);
        await repository.SaveChangesAsync(cancellationToken);
        return unit;
    }

    private static void RequireSouthAfricanScope(OrganizationUnit parent)
    {
        if (parent.CountryCode != "ZA")
            throw new InvalidOperationException("Only South African organization units are in the current build scope.");
    }
}
