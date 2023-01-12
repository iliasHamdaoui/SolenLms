using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.DependencyInjection;

public static class Infrastructure
{
    public static IServiceCollection AddCourseManagementInfrastructure(this IServiceCollection services, IConfiguration configuration, IMvcBuilder mvcBuilder)
    {
        services
       //.AddAuthorization()
        .AddPersistence(configuration)
        .AddWebApi(mvcBuilder);


        return services;
    }
}
