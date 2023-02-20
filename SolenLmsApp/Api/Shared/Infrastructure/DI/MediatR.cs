using FluentValidation;
using Imanys.SolenLms.Application.Shared.Infrastructure.MediatR.Pipelines;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class MediatR
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        Collection<Assembly> assemblies = new()
        {
            typeof(CourseManagement.IAssemblyReference).Assembly,
            typeof(Resources.IAssemblyReference).Assembly,
            typeof(Learning.IAssemblyReference).Assembly,
        };

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddFluentValidationRulesToSwagger();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies.ToArray()));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        return services;
    }
}
