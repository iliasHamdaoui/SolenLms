using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class Cors
{
    internal static IServiceCollection AddCors(this IServiceCollection services)
    {

        services.AddCors(options =>
        {
            options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });


        return services;
    }
}
