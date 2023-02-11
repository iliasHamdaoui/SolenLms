using Imanys.SolenLms.Application.Shared.Infrastructure.IdpEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class EventsListener
{
    public static IServiceCollection AddEventsListener(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<AzureServiceBusEventsListener>();
        services.Configure<AzureServiceBusEventsSettings>(configuration.GetSection("AzureServiceBusSettings"));

        return services;
    }
}