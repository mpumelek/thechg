using Microsoft.AspNetCore.Identity;

namespace TheChg.Infrastructure.Identity;

/// <summary>
/// An authentication account, not an official Church membership record.
/// Membership linkage and authorization scopes are separate, verified concepts.
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    // Required by the Identity user store and EF Core materialization.
    public ApplicationUser()
    {
    }

    public ApplicationUser(Guid churchId)
    {
        if (churchId == Guid.Empty)
        {
            throw new ArgumentException("A church is required for an account.", nameof(churchId));
        }

        Id = Guid.NewGuid();
        ChurchId = churchId;
    }

    public Guid ChurchId { get; private set; }

    public bool IsActive { get; private set; }

    internal void ActivateAfterVerifiedInvitation()
    {
        if (ChurchId == Guid.Empty || string.IsNullOrWhiteSpace(NormalizedEmail))
            throw new InvalidOperationException("An account needs a Church and verified email before activation.");

        EmailConfirmed = true;
        IsActive = true;
        SecurityStamp = Guid.NewGuid().ToString("N");
    }
}
