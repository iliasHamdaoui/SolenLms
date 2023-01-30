using Imanys.SolenLms.Application.Learning.Features;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.DependencyInjection;

internal static class Persistence
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {

        var dbConnectionString = configuration.GetConnectionString("SolenLmsAppDbConnection");

        services.AddDbContext<LearningDbContext>(options => options.UseSqlServer(dbConnectionString));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}
