using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.DependencyInjection;

internal static class WebApi
{
    internal static IServiceCollection AddWebApi(this IServiceCollection services, IMvcBuilder mvcBuilder)
    {

        mvcBuilder.AddApplicationPart(typeof(IAssemblyReference).Assembly);

        return services;
    }
}
