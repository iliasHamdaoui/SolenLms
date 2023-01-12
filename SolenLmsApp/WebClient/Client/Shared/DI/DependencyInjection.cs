using Blazored.Modal;
using Blazored.Toast;
using Fluxor;
using Imanys.SolenLms.Application.WebClient.Shared.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Plk.Blazor.DragDrop;
using Imanys.SolenLms.Application.WebClient.Shared.Layouts;
using Microsoft.AspNetCore.Components.Authorization;
using Scrutor;
using Imanys.SolenLms.Application.WebClient.Shared.Bff;
using Imanys.SolenLms.Application.WebClient.Shared.Authorization;
using System.Security.Claims;

namespace Imanys.SolenLms.Application.WebClient.Shared.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebClient(this IServiceCollection services, string apiAddress)
    {

        services.AddScoped<AuthenticationStateProvider, BffAuthenticationStateProvider>();


        services.AddAuthorizationCore(authorizationOptions =>
        {
            authorizationOptions.AddPolicy(PoliciesConstants.CourseManagementPolicy,
                configurePolicy => configurePolicy.RequireClaim(ClaimTypes.Role, "Admin", "Instructor"));

            authorizationOptions.AddPolicy(PoliciesConstants.AdminPolicy,
                configurePolicy => configurePolicy.RequireClaim(ClaimTypes.Role, "Admin"));
        });

        services.AddTransient<AntiforgeryHandler>();
        services.AddScoped<UnauthorizedMessageHandler>();

        services.AddLoadingBar();

        services.AddHttpClient("backend", client => client.BaseAddress = new Uri(apiAddress))
            .AddHttpMessageHandler<AntiforgeryHandler>()
            .AddHttpMessageHandler<UnauthorizedMessageHandler>();



        services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("backend").EnableIntercept(sp));


        services.AddBlazoredModal();
        services.AddBlazoredToast();
        services.AddBlazorDragDrop();

        services.AddScoped<NotificationsService>();

        var assemblies = new[] {
            typeof(MainLayout).Assembly,
        };


        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses()
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithScopedLifetime());

        services.AddFluxor(options =>
        {
            options.ScanAssemblies(assemblies.First(), assemblies);
#if DEBUG
            options.UseReduxDevTools();
#endif
        });

        return services;
    }
}