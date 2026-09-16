using Microsoft.EntityFrameworkCore;
using TheChg.Application.Authorization;
using TheChg.Domain.Organization;
using TheChg.Infrastructure.Organization;

namespace TheChg.Infrastructure.Authorization;

/// <summary>Validates the entire country/circuit/branch chain, failing closed on corruption.</summary>
public sealed class EfOrganizationAncestryReader(OrganizationDbContext organization) : IOrganizationAncestryReader
{
    public async Task<IReadOnlyList<OrganizationAncestor>?> FindInclusiveAncestryAsync(
        Guid organizationUnitId, CancellationToken cancellationToken)
    {
        if (organizationUnitId == Guid.Empty)
            return null;

        var chain = new List<OrganizationAncestor>(3);
        var seen = new HashSet<Guid>();
        var currentId = organizationUnitId;
        Guid? churchId = null;
        OrganizationUnitType? childType = null;

        for (var depth = 0; depth < 3; depth++)
        {
            if (!seen.Add(currentId))
                return null;

            var unit = await organization.Units.AsNoTracking()
                .Where(candidate => candidate.Id == currentId)
                .Select(candidate => new { candidate.Id, candidate.ChurchId, candidate.ParentId, candidate.UnitType })
                .SingleOrDefaultAsync(cancellationToken);
            if (unit is null || unit.ChurchId == Guid.Empty ||
                (churchId is not null && unit.ChurchId != churchId) ||
                (childType is not null && (int)unit.UnitType != (int)childType.Value - 1))
                return null;

            churchId = unit.ChurchId;
            chain.Add(new OrganizationAncestor(unit.Id, unit.ChurchId));

            if (unit.UnitType == OrganizationUnitType.Country)
            {
                if (unit.ParentId is not null)
                    return null;
                return await organization.Churches.AsNoTracking()
                    .AnyAsync(church => church.Id == churchId, cancellationToken) ? chain : null;
            }

            if (unit.ParentId is not Guid parentId || parentId == Guid.Empty ||
                unit.UnitType is not (OrganizationUnitType.Branch or OrganizationUnitType.Circuit))
                return null;

            childType = unit.UnitType;
            currentId = parentId;
        }

        return null;
    }
}
