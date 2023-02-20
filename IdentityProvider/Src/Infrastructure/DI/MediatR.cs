using FluentValidation;
using Imanys.SolenLms.IdentityProvider.Infrastructure.MediatR;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class MediatR
{
    public static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        var assemblies = new Collection<Assembly>
        {
            typeof(Core.IAssemblyReference).Assembly
        };

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddFluentValidationRulesToSwagger();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies.ToArray()));


        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));



        return services;
    }
}
