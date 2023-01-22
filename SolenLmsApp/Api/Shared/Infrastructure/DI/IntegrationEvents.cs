using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Infrastructure.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

#nullable disable

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class IntegrationEvents
{
    public static IServiceCollection AddIntegrationEvents(this IServiceCollection services)
    {
        services.AddSingleton<IIntegrationEventsSender, IntegrationEventsBroker>();

        services.AddHostedService(sp =>
            sp.GetRequiredService<IIntegrationEventsSender>() as IntegrationEventsBroker);
        
        return services;
    }
}