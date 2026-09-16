using Microsoft.EntityFrameworkCore;
using TheChg.Application.Organization;
using TheChg.Domain.Organization;

namespace TheChg.Infrastructure.Organization;

public sealed class EfOrganizationRepository(OrganizationDbContext dbContext) : IOrganizationRepository
{
    public Task<Church?> FindChurchAsync(Guid churchId, CancellationToken cancellationToken) =>
        dbContext.Churches.AsNoTracking().SingleOrDefaultAsync(church => church.Id == churchId, cancellationToken);

    public Task<OrganizationUnit?> FindUnitAsync(Guid unitId, CancellationToken cancellationToken) =>
        dbContext.Units.AsNoTracking().SingleOrDefaultAsync(unit => unit.Id == unitId, cancellationToken);

    public Task<bool> CodeExistsAsync(Guid churchId, Guid? parentId, string code, CancellationToken cancellationToken) =>
        dbContext.Units.AnyAsync(unit => unit.ChurchId == churchId && unit.ParentId == parentId && unit.Code == code,
            cancellationToken);

    public void Add(OrganizationUnit unit) => dbContext.Units.Add(unit);

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
