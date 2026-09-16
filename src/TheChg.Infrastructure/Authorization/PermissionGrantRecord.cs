namespace TheChg.Infrastructure.Authorization;

/// <summary>A persisted, explicit permission assignment. This is not an Identity role.</summary>
public sealed class PermissionGrantRecord
{
    private PermissionGrantRecord() { } // EF Core

    private PermissionGrantRecord(Guid id, Guid accountId, Guid churchId, string permission,
        Guid? scopeUnitId, bool includeDescendants, DateTimeOffset effectiveFrom, DateTimeOffset? effectiveUntil)
    {
        Id = id;
        AccountId = accountId;
        ChurchId = churchId;
        Permission = permission;
        ScopeUnitId = scopeUnitId;
        IncludeDescendants = includeDescendants;
        EffectiveFrom = effectiveFrom;
        EffectiveUntil = effectiveUntil;
    }

    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid ChurchId { get; private set; }
    public string Permission { get; private set; } = null!;
    public Guid? ScopeUnitId { get; private set; }
    public bool IncludeDescendants { get; private set; }
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveUntil { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public static PermissionGrantRecord Create(Guid id, Guid accountId, Guid churchId, string permission,
        Guid? scopeUnitId, bool includeDescendants, DateTimeOffset effectiveFrom, DateTimeOffset? effectiveUntil)
    {
        if (id == Guid.Empty || accountId == Guid.Empty || churchId == Guid.Empty || scopeUnitId == Guid.Empty)
            throw new ArgumentException("Grant, account, church and non-null scope identifiers must be non-empty.");
        if (string.IsNullOrWhiteSpace(permission) || permission.Length > 128 || permission != permission.Trim())
            throw new ArgumentException("A canonical permission of at most 128 characters is required.", nameof(permission));
        if (effectiveUntil is not null && effectiveUntil <= effectiveFrom)
            throw new ArgumentException("Grant end must follow grant start.", nameof(effectiveUntil));
        if (scopeUnitId is null && includeDescendants)
            throw new ArgumentException("A church-wide grant cannot have a descendant flag.", nameof(includeDescendants));

        return new PermissionGrantRecord(id, accountId, churchId, permission, scopeUnitId,
            includeDescendants, effectiveFrom, effectiveUntil);
    }

    public void Revoke(DateTimeOffset revokedAt)
    {
        if (revokedAt == default)
            throw new ArgumentException("A revocation timestamp is required.", nameof(revokedAt));
        if (RevokedAt is not null)
            throw new InvalidOperationException("The grant has already been revoked.");
        RevokedAt = revokedAt;
    }
}
