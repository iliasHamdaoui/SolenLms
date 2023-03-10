using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class Infrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mvcBuilder = services.AddMvc(configuration);

        services.AddAspNetIdentity(configuration)
            .AddExternalProviders()
            .AddIdentityServer(configuration)
            .AddWebConfiguration()
            .AddMediatR()
            .AddWebApi(mvcBuilder)
            .AddCors()
            .AddOpenApi()
            .AddServices(configuration)
            .AddMessaging(configuration);

        services.Configure<SolenLmsWebClientUrl>(options => options.Value = configuration["WebClient:Url"]!.Split(';').First());

        return services;
    }
}
