using FluentValidation;
using Imanys.SolenLms.Application.Shared.Infrastructure.MediatR.Pipelines;
using MediatR;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class MediatR
{
    public static IServiceCollection AddMediatR(this IServiceCollection services, IMvcBuilder mvcBuilder)
    {
        var assemblies = new Collection<Assembly>
        {
            typeof(CourseManagement.Core.UseCases.IAssemblyReference).Assembly,
            typeof(CourseManagement.Infrastructure.IAssemblyReference).Assembly,
            typeof(Resources.Core.UseCases.IAssemblyReference).Assembly,
            typeof(Resources.Infrastructure.IAssemblyReference).Assembly,
            typeof(Learning.Core.UseCases.IAssemblyReference).Assembly,
            typeof(Learning.Infrastructure.IAssemblyReference).Assembly,
        };

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddFluentValidationRulesToSwagger();

        services.AddMediatR(assemblies.ToArray());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

        return services;
    }
}
