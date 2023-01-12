using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Infrastructure.Services;
using Imanys.SolenLms.Application.Shared.Infrastructure.Services.AzureServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#nullable disable

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Services
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IIntegratedEventsSender, IntegratedEventsBrokerServices>();


        services.AddHostedService(sp =>
            sp.GetRequiredService<IIntegratedEventsSender>() as IntegratedEventsBrokerServices);


        services.AddHostedService<IdpEventsListenerService>();

        services.AddSingleton(_ => new ServiceBusClient(configuration["AzureServiceBusSettings:ConnectionString"]));
        services.AddSingleton(_ => new IdpEventsCreatorFactory());
        services.Configure<AzureServiceBusSettings>(configuration.GetSection("AzureServiceBusSettings"));

        return services;
    }
}