using TheChg.Application.Authorization;
using TheChg.Contracts.Organization;
using TheChg.Domain.Organization;

namespace TheChg.Application.Organization;

/// <summary>Loads the branch first so authorization uses trusted stored scope metadata.</summary>
public sealed class BranchDetailsService(
    IOrganizationRepository repository,
    ScopedAuthorizationEvaluator authorization)
{
    public async Task<BranchDetailsResponse?> GetAsync(
        Guid accountId, Guid branchId, CancellationToken cancellationToken = default)
    {
        if (accountId == Guid.Empty || branchId == Guid.Empty)
            return null;

        var branch = await repository.FindUnitAsync(branchId, cancellationToken);
        if (branch is null || branch.UnitType != OrganizationUnitType.Branch ||
            branch.ParentId is not Guid circuitId || branch.CountryCode != "ZA")
            return null;

        var resource = new AuthorizationResource(branch.ChurchId, branch.Id);
        if (!await authorization.IsAllowedAsync(accountId, "Organization.View", resource, cancellationToken))
            return null;

        return new BranchDetailsResponse(branch.Id, branch.Code, branch.Name, circuitId, branch.CountryCode);
    }
}
