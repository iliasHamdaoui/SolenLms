using Imanys.SolenLms.Application.Resources.Infrastructure.VideoDurationCalculator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;

internal static class VideoDurationCalculator
{
    public static IServiceCollection AddVideoDurationCalculator(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<VideoDurationCalculatorListenerService>();

        services.Configure<VideoDurationCalculatorAzureServiceBusSettings>(
            configuration.GetSection("AzureServiceBusSettings"));

        return services;
    }
}