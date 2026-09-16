namespace TheChg.Infrastructure.Identity;

/// <summary>Only a token digest is persisted. An invitation never grants an organization permission.</summary>
public sealed class AccountInvitation
{
    private AccountInvitation() { } // EF Core

    internal AccountInvitation(Guid userId, Guid churchId, string normalizedDestination,
        string tokenHash, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ChurchId = churchId;
        NormalizedDestination = normalizedDestination;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ChurchId { get; private set; }
    public string NormalizedDestination { get; private set; } = null!;
    public string TokenHash { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
}
