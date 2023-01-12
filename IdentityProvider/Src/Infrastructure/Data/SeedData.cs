using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Data;

public static class SeedData
{
    public static void EnsureSeedData(WebApplication app, IConfiguration configuration)
    {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

        ConfigurationDbContext configurationDbContext =
            scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

#if DEBUG

        scope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();

        scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

        configurationDbContext.Database.Migrate();

#endif


        if (!configurationDbContext.Clients.Any())
        {
            foreach (Client client in Config.Clients(configuration))
                configurationDbContext.Clients.Add(client.ToEntity());
        }

        if (!configurationDbContext.IdentityResources.Any())
        {
            foreach (IdentityResource resource in Config.IdentityResources)
                configurationDbContext.IdentityResources.Add(resource.ToEntity());
        }

        if (!configurationDbContext.ApiScopes.Any())
        {
            foreach (ApiScope apiScope in Config.ApiScopes)
                configurationDbContext.ApiScopes.Add(apiScope.ToEntity());
        }


        configurationDbContext.SaveChanges();
    }
}