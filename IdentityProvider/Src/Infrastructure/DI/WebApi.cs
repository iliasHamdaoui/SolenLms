using Imanys.SolenLms.IdentityProvider.WebApi;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class WebApi
{
    internal static IServiceCollection AddWebApi(this IServiceCollection services, IMvcBuilder mvcBuilder)
    {

        mvcBuilder.AddApplicationPart(typeof(IAssemblyReference).Assembly);

        return services;
    }
}
