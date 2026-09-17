using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheChg.Application.Registration;

namespace TheChg.Infrastructure.Identity;

public sealed class EfAccountRegistrationWriter(
    TheChgIdentityDbContext context,
    UserManager<ApplicationUser> users,
    TimeProvider clock) : IAccountRegistrationWriter
{
    public async Task<PendingAccountRegistration> CreatePendingAsync(Guid registrarId, Guid churchId,
        Guid branchId, BranchAccountKind kind, string email, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        var account = new ApplicationUser(churchId) { UserName = email, Email = email };
        var created = await users.CreateAsync(account);
        if (!created.Succeeded)
        {
            if (created.Errors.Any(error => error.Code is "DuplicateEmail" or "DuplicateUserName"))
                throw new DuplicateAccountRegistrationException();
            throw new InvalidOperationException("The pending account could not be created.");
        }

        var registration = new BranchAccountRegistration(registrarId, account.Id, churchId,
            branchId, kind, clock.GetUtcNow().UtcDateTime);
        context.BranchAccountRegistrations.Add(registration);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new PendingAccountRegistration(registration.Id, account.Id, branchId, kind);
    }
}
