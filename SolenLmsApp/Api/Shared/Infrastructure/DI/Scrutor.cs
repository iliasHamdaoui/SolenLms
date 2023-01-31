using Imanys.SolenLms.Application.Resources.Features;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Scrutor
{
    public static IServiceCollection AddScrutor(this IServiceCollection services)
    {
        Collection<Assembly> assemblies = new()
        {
            typeof(CourseManagement.IAssemblyReference).Assembly,
            typeof(Resources.IAssemblyReference).Assembly,
            typeof(Learning.IAssemblyReference).Assembly,
        };

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(x => x.Where(type => type.Name != nameof(ResourceFile)), publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithScopedLifetime());

        return services;
    }
}