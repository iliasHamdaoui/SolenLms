using Imanys.SolenLms.Application.CourseManagement.WebApi.Controllers.Courses;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.DependencyInjection;

internal static class WebApi
{
    internal static IServiceCollection AddWebApi(this IServiceCollection services, IMvcBuilder mvcBuilder)
    {

        mvcBuilder.AddApplicationPart(typeof(CoursesController).GetTypeInfo().Assembly);

        return services;
    }
}
