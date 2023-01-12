using Imanys.SolenLms.Application.Resources.WebApi.Controllers.Lectures.Instructor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;

internal static class WebApi
{
    internal static IServiceCollection AddWebApi(this IServiceCollection services, IMvcBuilder mvcBuilder)
    {

        mvcBuilder.AddApplicationPart(typeof(ResourcesController).GetTypeInfo().Assembly);

        return services;
    }
}
