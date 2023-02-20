using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Infrastructure.AzureServiceBus;
using Imanys.SolenLms.Application.Shared.Infrastructure.MediatR;
using Imanys.SolenLms.Application.Shared.Infrastructure.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#nullable disable

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;


internal static class Messaging
{
    public static IServiceCollection AddMessaging(this IServiceCollection services,
        IConfiguration configuration)
    {


        services.AddSingleton<IIntegrationEventsSender, IntegrationEventsBroker>();

        services.AddHostedService(sp =>
            sp.GetRequiredService<IIntegrationEventsSender>() as IntegrationEventsBroker);


        services.AddHostedService<AzureServiceBusEventsListener>();
        services.AddSingleton(_ => new ServiceBusClient(configuration["AzureServiceBusSettings:ConnectionString"]));

        if (configuration["RabbitMqSettings:UseRabbitMq"] != null && bool.Parse(configuration["RabbitMqSettings:UseRabbitMq"]))
        {
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"));
            services.AddHostedService<RabbitMqIntegrationEventListener>();
        }

        return services;
    }
}
