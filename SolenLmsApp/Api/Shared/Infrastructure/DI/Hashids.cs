using HashidsNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class HashidsPackage
{
    public static IServiceCollection AddHashids(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHashids>(_ => new Hashids(configuration["Hashids:Salt"], int.Parse(configuration["Hashids:MinHashLength"]!), configuration["Hashids:Alphabet"]));
        return services;
    }
}
