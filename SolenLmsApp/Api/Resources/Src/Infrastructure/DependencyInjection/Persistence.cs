using Imanys.SolenLms.Application.Resources.Features;
using Imanys.SolenLms.Application.Resources.Features.Lectures.Commands.UploadLectureVideo;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data.Repositories.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;

internal static class Persistence
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {

        var dbConnectionString = configuration.GetConnectionString("SolenLmsAppDbConnection");

        services.AddDbContext<ResourcesDbContext>(options => options.UseSqlServer(dbConnectionString));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IStorageRepo, StorageRepo>();

        return services;
    }
}