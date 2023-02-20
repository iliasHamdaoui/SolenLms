using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.IdentityProvider.Infrastructure.RabbitMq;
using Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


#nullable disable

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;
internal static class Messaging
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {

        if (configuration["RabbitMqSettings:UseRabbitMq"] != null && bool.Parse(configuration["RabbitMqSettings:UseRabbitMq"]))
        {
            services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMqSettings"));

            services.AddSingleton<IIntegrationEventsSender, RabbitMqIntegrationEventBroker>();
            services.AddHostedService(sp => sp.GetRequiredService<IIntegrationEventsSender>() as RabbitMqIntegrationEventBroker);
        }
        else
        {
            services.AddSingleton<IIntegrationEventsSender, AzureBusIntegrationEventBroker>();
            services.AddHostedService(sp => sp.GetRequiredService<IIntegrationEventsSender>() as AzureBusIntegrationEventBroker);


            services.AddSingleton(_ => new ServiceBusClient(configuration["AzureServiceBusSettings:ConnectionString"]));
            services.Configure<AzureServiceBusSettings>(configuration.GetSection("AzureServiceBusSettings"));
        }


        return services;
    }
}
