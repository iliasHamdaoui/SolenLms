using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.DependencyInjection;

internal static class WebApi
{
    internal static IServiceCollection AddWebApi(this IServiceCollection services, IMvcBuilder mvcBuilder)
    {
        mvcBuilder.AddApplicationPart(typeof(Imanys.SolenLms.Application.CourseManagement.IAssemblyReference)
            .GetTypeInfo().Assembly);

        return services;
    }
}