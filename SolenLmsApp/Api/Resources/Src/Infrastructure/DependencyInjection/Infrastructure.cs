using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;

public static class Infrastructure
{
    public static IServiceCollection AddResourcesInfrastructure(this IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder, IWebHostEnvironment environment)
    {
        services
        .AddPersistence(configuration)
        .AddStorage(configuration)
        .AddWebApi(mvcBuilder);


        return services;
    }
}