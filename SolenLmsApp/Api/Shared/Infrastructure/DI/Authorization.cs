using Microsoft.Extensions.DependencyInjection;
using static Imanys.SolenLms.Application.Shared.WebApi.PoliciesConstants;
using static Imanys.SolenLms.Application.Shared.Core.Enums.UserRole;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Authorization
{
    internal static IServiceCollection AddAuthorization(this IServiceCollection services)
    {

        services.AddAuthorization(authorizationOptions =>
        {
            authorizationOptions.AddPolicy(CourseManagementPolicy,
              configurePolicy => configurePolicy.RequireRole(Instructor, Admin));

            authorizationOptions.AddPolicy(AdminPolicy,
                configurePolicy => configurePolicy.RequireRole(Admin));
        });

        return services;
    }
}
