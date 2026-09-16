using TheChg.Domain.Organization;

namespace TheChg.Application.Organization;

public interface IOrganizationRepository
{
    Task<Church?> FindChurchAsync(Guid churchId, CancellationToken cancellationToken);
    Task<OrganizationUnit?> FindUnitAsync(Guid unitId, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(Guid churchId, Guid? parentId, string code, CancellationToken cancellationToken);
    void Add(OrganizationUnit unit);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
