using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.DependencyInjection;

public static class Infrastructure
{
    public static IServiceCollection AddLearnerInfrastructure(this IServiceCollection services,
        IConfiguration configuration, IMvcBuilder mvcBuilder)
    {
        services
            .AddPersistence(configuration)
            .AddRepositories()
            .AddWebApi(mvcBuilder);


        return services;
    }
}