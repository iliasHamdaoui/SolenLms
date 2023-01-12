using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Infrastructure.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Security
{
    internal static IServiceCollection AddSecurity(this IServiceCollection services)
    {

        services.AddCors(options =>
        {
            options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        });

        services.AddScoped<ICurrentUser, CurrentUser>();

        return services;
    }
}
