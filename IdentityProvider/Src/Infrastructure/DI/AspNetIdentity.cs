using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Imanys.SolenLms.IdentityProvider.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class AspNetIdentity
{
    internal static IServiceCollection AddAspNetIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        string? dbConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(dbConnectionString));

        return services;
    }
}