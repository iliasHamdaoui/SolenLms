using Imanys.SolenLms.Application.Shared.Infrastructure.IdpEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;



internal static class IdpEvents
{
    public static IServiceCollection AddIdpEvents(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddHostedService<IdpEventsListenerService>();
        services.AddSingleton(_ => new IdpEventsCreatorFactory());
        services.Configure<IdpEventsAzureServiceBusSettings>(configuration.GetSection("AzureServiceBusSettings"));

        return services;
    }
}