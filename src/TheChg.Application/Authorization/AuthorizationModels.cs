namespace TheChg.Application.Authorization;

/// <summary>Account state supplied by the identity adapter, not by a request payload.</summary>
public sealed record AccessAccount(Guid Id, Guid ChurchId, bool IsActive);

/// <summary>
/// A single effective-dated permission assignment. A null scope unit means the entire church;
/// descendant access for a unit assignment must be explicitly enabled.
/// </summary>
public sealed record PermissionGrant(
    Guid AccountId,
    Guid ChurchId,
    string Permission,
    Guid? ScopeUnitId,
    bool IncludeDescendants,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveUntil);

/// <summary>Loaded resource metadata. Callers must derive it from the stored resource, not request IDs.</summary>
public sealed record AuthorizationResource(
    Guid ChurchId,
    Guid? OrganizationUnitId,
    Guid? RecordId = null,
    bool IsSensitive = false);

/// <summary>One node of the target unit's inclusive ancestry, starting at the target.</summary>
public sealed record OrganizationAncestor(Guid Id, Guid ChurchId);

public interface IAccessAccountReader
{
    Task<AccessAccount?> FindAsync(Guid accountId, CancellationToken cancellationToken);
}

public interface IPermissionGrantReader
{
    Task<IReadOnlyCollection<PermissionGrant>> FindForAccountAsync(
        Guid accountId, Guid churchId, CancellationToken cancellationToken);
}

public interface IOrganizationAncestryReader
{
    /// <summary>Returns an inclusive target-to-root chain, or null when the unit cannot be resolved.</summary>
    Task<IReadOnlyList<OrganizationAncestor>?> FindInclusiveAncestryAsync(
        Guid organizationUnitId, CancellationToken cancellationToken);
}

/// <summary>
/// An additional record-specific policy. No implementation is supplied by default; sensitive
/// resources are denied until a use case deliberately provides and tests one.
/// </summary>
public interface ISensitiveRecordAccessPolicy
{
    Task<bool> IsAllowedAsync(
        Guid accountId, string permission, AuthorizationResource resource, CancellationToken cancellationToken);
}
