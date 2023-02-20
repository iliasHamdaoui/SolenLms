using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#nullable disable
namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class Services
{
    internal static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IOrganizationService, OrganizationService>();

        services.AddSingleton<IEmailService, EmailBackgroundService>();
        services.AddHostedService(sp => sp.GetRequiredService<IEmailService>() as EmailBackgroundService);
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));


        return services;
    }
}
