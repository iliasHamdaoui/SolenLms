using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class AzureServiceBus
{
    public static IServiceCollection AddAzureServiceBus(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(_ => new ServiceBusClient(configuration["AzureServiceBusSettings:ConnectionString"]));
        return services;
    }
}