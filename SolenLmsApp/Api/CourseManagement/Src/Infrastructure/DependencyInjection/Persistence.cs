using Imanys.SolenLms.Application.CourseManagement.Core.UseCases;
using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.DependencyInjection;

internal static class Persistence
{
    internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {

        string? dbConnectionString = configuration.GetConnectionString("SolenLmsAppDbConnection");

        services.AddDbContext<CourseManagementDbContext>(options => options.UseSqlServer(dbConnectionString));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        return services;
    }
}
