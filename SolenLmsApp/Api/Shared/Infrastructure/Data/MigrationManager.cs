using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.Data;

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication webApp)
    {
        using (var scope = webApp.Services.CreateScope())
        {
            using var instructorDbContext = scope.ServiceProvider.GetRequiredService<CourseManagementDbContext>();
            using var resourceDbContext = scope.ServiceProvider.GetRequiredService<ResourcesDbContext>();
            using var learnerDbContext = scope.ServiceProvider.GetRequiredService<LearningDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(nameof(MigrationManager));
            try
            {
                instructorDbContext.Database.Migrate();
                resourceDbContext.Database.Migrate();
                learnerDbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
        return webApp;
    }
}
