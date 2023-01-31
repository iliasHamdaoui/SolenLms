using Imanys.SolenLms.Application.CourseManagement.Infrastructure.DependencyInjection;
using Imanys.SolenLms.Application.Learning.Infrastructure.DependencyInjection;
using Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        IMvcBuilder mvcBuilder = services.AddMvc(configuration);

        services
        .AddMediatR()
        .AddSwagger()
        .AddAppAuthentication(configuration, environment)
        .AddAuthorization()
        .AddSystem()
        .AddLocalization()
        .AddIntegrationEvents()
        .AddIdpEvents(configuration)
        .AddAzureServiceBus(configuration)
        .AddSecurity()
        .AddHashids(configuration)
        .AddCourseManagementInfrastructure(configuration, mvcBuilder)
        .AddResourcesInfrastructure(configuration, mvcBuilder)
        .AddLearnerInfrastructure(configuration, mvcBuilder)
        .AddScrutor();

        return services;
    }
}
