namespace TheChg.Application.Authorization;

/// <summary>
/// Evaluates a loaded resource against current account state and grants. A role name alone never
/// authorizes a request. Readers must query current state; callers must not cache allow decisions.
/// </summary>
public sealed class ScopedAuthorizationEvaluator(
    IAccessAccountReader accounts,
    IPermissionGrantReader grants,
    IOrganizationAncestryReader ancestry,
    TimeProvider clock,
    ISensitiveRecordAccessPolicy? sensitiveRecords = null)
{
    public async Task<bool> IsAllowedAsync(
        Guid accountId,
        string permission,
        AuthorizationResource resource,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resource);

        if (accountId == Guid.Empty || resource.ChurchId == Guid.Empty ||
            string.IsNullOrWhiteSpace(permission) || permission != permission.Trim() ||
            resource.OrganizationUnitId == Guid.Empty || resource.RecordId == Guid.Empty ||
            (resource.IsSensitive && resource.RecordId is null))
            return false;

        var account = await accounts.FindAsync(accountId, cancellationToken);
        if (account is null || account.Id != accountId || account.ChurchId != resource.ChurchId ||
            !account.IsActive)
            return false;

        IReadOnlyList<OrganizationAncestor>? chain = null;
        if (resource.OrganizationUnitId is Guid unitId)
        {
            chain = await ancestry.FindInclusiveAncestryAsync(unitId, cancellationToken);
            if (chain is null || chain.Count == 0 || chain[0].Id != unitId ||
                chain.Any(node => node.Id == Guid.Empty || node.ChurchId != resource.ChurchId) ||
                chain.Select(node => node.Id).Distinct().Count() != chain.Count)
                return false;
        }

        var currentGrants = await grants.FindForAccountAsync(accountId, resource.ChurchId, cancellationToken);
        if (currentGrants is null)
            return false;

        var now = clock.GetUtcNow();
        var matchingGrant = currentGrants.Any(grant =>
            grant.AccountId == accountId &&
            grant.ChurchId == resource.ChurchId &&
            string.Equals(grant.Permission, permission, StringComparison.Ordinal) &&
            grant.EffectiveFrom <= now &&
            (grant.EffectiveUntil is null || now < grant.EffectiveUntil) &&
            (grant.ScopeUnitId is null ||
             (chain is not null &&
              (chain[0].Id == grant.ScopeUnitId ||
               (grant.IncludeDescendants && chain.Skip(1).Any(node => node.Id == grant.ScopeUnitId))))));

        if (!matchingGrant)
            return false;

        if (!resource.IsSensitive)
            return true;

        return sensitiveRecords is not null &&
            await sensitiveRecords.IsAllowedAsync(accountId, permission, resource, cancellationToken);
    }
}
