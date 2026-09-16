using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheChg.Application.Organization;

namespace TheChg.Infrastructure.Organization;

public static class OrganizationServiceCollectionExtensions
{
    public static IServiceCollection AddOrganizationPersistence(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        services.AddDbContext<OrganizationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();
        services.AddScoped<OrganizationHierarchyService>();
        return services;
    }
}
