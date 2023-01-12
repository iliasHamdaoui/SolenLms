using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Imanys.SolenLms.Application.Shared.Infrastructure.System;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class System
{
    public static IServiceCollection AddSystem(this IServiceCollection services)
    {
        services.AddTransient<IDateTime, MachineDateTime>();
        return services;
    }
}
