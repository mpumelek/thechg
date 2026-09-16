using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheChg.Application.Authorization;

namespace TheChg.Infrastructure.Authorization;

public static class AuthorizationServiceCollectionExtensions
{
    /// <summary>Registers persisted grant and account/scope readers, not grant administration.</summary>
    public static IServiceCollection AddAuthorizationPersistence(this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<AuthorizationDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAccessAccountReader, EfAccessAccountReader>();
        services.AddScoped<IPermissionGrantReader, EfPermissionGrantReader>();
        services.AddScoped<IOrganizationAncestryReader, EfOrganizationAncestryReader>();
        return services;
    }
}
