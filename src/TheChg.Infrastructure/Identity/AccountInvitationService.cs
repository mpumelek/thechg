using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TheChg.Infrastructure.Identity;

/// <summary>
/// Internal provisioning primitive. Callers must independently authorize invitation issuance,
/// deliver the secret out of band. Redemption enrolls credentials but grants no scope.
/// </summary>
public sealed class AccountInvitationService(TheChgIdentityDbContext context,
    UserManager<ApplicationUser> userManager, TimeProvider clock)
{
    public async Task<string> IssueAsync(Guid accountId, string destination,
        TimeSpan lifetime, CancellationToken cancellationToken = default)
    {
        if (accountId == Guid.Empty || lifetime <= TimeSpan.Zero || lifetime > TimeSpan.FromDays(7))
            throw new ArgumentException("A valid account and lifetime of at most seven days are required.");

        var normalizedDestination = NormalizeDestination(destination);
        var account = await context.Users.SingleOrDefaultAsync(user => user.Id == accountId, cancellationToken)
            ?? throw new InvalidOperationException("Account not found.");
        if (account.IsActive || account.PasswordHash is not null || account.ChurchId == Guid.Empty ||
            !string.Equals(account.NormalizedEmail, normalizedDestination, StringComparison.Ordinal))
            throw new InvalidOperationException("Account is not eligible for an invitation to this destination.");

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        context.Invitations.Add(new AccountInvitation(account.Id, account.ChurchId,
            normalizedDestination, HashToken(token), clock.GetUtcNow().Add(lifetime).UtcDateTime));
        await context.SaveChangesAsync(cancellationToken);
        return token;
    }

    public async Task<bool> RedeemAsync(string token, string destination, string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length > 256 || string.IsNullOrEmpty(password))
            return false;

        string normalizedDestination;
        try { normalizedDestination = NormalizeDestination(destination); }
        catch (ArgumentException) { return false; }

        var tokenHash = HashToken(token);
        var now = clock.GetUtcNow().UtcDateTime;
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var invitation = await context.Invitations.AsNoTracking()
            .SingleOrDefaultAsync(candidate => candidate.TokenHash == tokenHash, cancellationToken);
        if (invitation is null || invitation.ConsumedAt is not null || invitation.ExpiresAt <= now ||
            !string.Equals(invitation.NormalizedDestination, normalizedDestination, StringComparison.Ordinal))
            return false;

        var account = await context.Users.SingleOrDefaultAsync(user => user.Id == invitation.UserId,
            cancellationToken);
        if (account is null || account.IsActive || account.PasswordHash is not null ||
            account.ChurchId != invitation.ChurchId ||
            !string.Equals(account.NormalizedEmail, normalizedDestination, StringComparison.Ordinal))
            return false;

        // Predicate makes consumption atomic even when two requests redeem concurrently.
        var consumed = await context.Invitations
            .Where(candidate => candidate.Id == invitation.Id && candidate.ConsumedAt == null &&
                candidate.ExpiresAt > now)
            .ExecuteUpdateAsync(setters => setters.SetProperty(candidate => candidate.ConsumedAt, now),
                cancellationToken);
        if (consumed != 1)
            return false;

        var passwordResult = await userManager.AddPasswordAsync(account, password);
        if (!passwordResult.Succeeded)
            return false;

        account.ActivateAfterVerifiedInvitation();
        var activationResult = await userManager.UpdateAsync(account);
        if (!activationResult.Succeeded)
            return false;
        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    private static string NormalizeDestination(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination) || destination.Trim().Length > 256 ||
            !destination.Contains('@', StringComparison.Ordinal))
            throw new ArgumentException("A valid invitation email destination is required.", nameof(destination));
        return destination.Trim().ToUpperInvariant();
    }

    private static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
