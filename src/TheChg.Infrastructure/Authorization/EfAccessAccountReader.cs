using Microsoft.EntityFrameworkCore;
using TheChg.Application.Authorization;
using TheChg.Infrastructure.Identity;

namespace TheChg.Infrastructure.Authorization;

public sealed class EfAccessAccountReader(TheChgIdentityDbContext identity) : IAccessAccountReader
{
    public async Task<AccessAccount?> FindAsync(Guid accountId, CancellationToken cancellationToken)
    {
        if (accountId == Guid.Empty)
            return null;

        return await identity.Users.AsNoTracking()
            .Where(account => account.Id == accountId)
            .Select(account => new AccessAccount(account.Id, account.ChurchId,
                account.IsActive && account.EmailConfirmed))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
