using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class Web
{
    public static IServiceCollection AddWebConfiguration(this IServiceCollection services)
    {
        services.AddRazorPages();

        return services;
    }
}
