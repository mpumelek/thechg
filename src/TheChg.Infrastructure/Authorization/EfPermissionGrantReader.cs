using Microsoft.EntityFrameworkCore;
using TheChg.Application.Authorization;
using TheChg.Infrastructure.Organization;

namespace TheChg.Infrastructure.Authorization;

/// <summary>Loads current persisted grants; revoked and cross-Church scopes never authorize.</summary>
public sealed class EfPermissionGrantReader(
    AuthorizationDbContext authorization,
    OrganizationDbContext organization) : IPermissionGrantReader
{
    public async Task<IReadOnlyCollection<PermissionGrant>> FindForAccountAsync(
        Guid accountId, Guid churchId, CancellationToken cancellationToken)
    {
        if (accountId == Guid.Empty || churchId == Guid.Empty ||
            !await organization.Churches.AsNoTracking().AnyAsync(church => church.Id == churchId, cancellationToken))
            return [];

        var records = await authorization.PermissionGrants.AsNoTracking()
            .Where(grant => grant.AccountId == accountId && grant.ChurchId == churchId && grant.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var scopeIds = records.Where(record => record.ScopeUnitId is not null)
            .Select(record => record.ScopeUnitId!.Value).Distinct().ToArray();
        var validScopes = await organization.Units.AsNoTracking()
            .Where(unit => unit.ChurchId == churchId && scopeIds.Contains(unit.Id))
            .Select(unit => unit.Id).ToListAsync(cancellationToken);
        var validScopeSet = validScopes.ToHashSet();

        return records
            .Where(record => record.ScopeUnitId is null || validScopeSet.Contains(record.ScopeUnitId.Value))
            .Select(record => new PermissionGrant(record.AccountId, record.ChurchId, record.Permission,
                record.ScopeUnitId, record.IncludeDescendants, record.EffectiveFrom, record.EffectiveUntil))
            .ToArray();
    }
}
