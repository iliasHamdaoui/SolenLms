using Duende.IdentityServer;
using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using static Imanys.SolenLms.Application.Shared.Core.Enums.UserRole;
using static Imanys.SolenLms.IdentityProvider.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class IdentityServer
{
    public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        var identityBuilder = services
        .AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;

            // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
            options.EmitStaticAudienceClaim = true;
        });

        identityBuilder.AddAspNetIdentity<User>();


        var migrationsAssembly = typeof(IdentityServer).Assembly.GetName().Name;

        var idpDataDbConnectionString = configuration.GetConnectionString("DefaultConnection");

        identityBuilder.AddConfigurationStore(options =>
        {
            options.ConfigureDbContext = dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(idpDataDbConnectionString,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
        });

        identityBuilder.AddOperationalStore(options =>
        {
            options.ConfigureDbContext = dbContextOptionsBuilder =>
                dbContextOptionsBuilder.UseSqlServer(idpDataDbConnectionString,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(migrationsAssembly));
        });

        services.AddLocalApiAuthentication();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthenticatedUserPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdminPolicy, policy =>
            {
                policy.AddAuthenticationSchemes(IdentityServerConstants.LocalApi.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.Role, Admin);
            });
        });

        return services;
    }
}