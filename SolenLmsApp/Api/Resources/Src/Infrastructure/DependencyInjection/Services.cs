using Imanys.SolenLms.Application.Resources.Infrastructure.Services.VideoDurationCalculator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;

internal static class Services
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<VideoDurationCalculatorListenerService>();

        services.Configure<VideoDurationCalculatorAzureServiceBusSettings>(
            configuration.GetSection("AzureServiceBusSettings"));

        return services;
    }
}