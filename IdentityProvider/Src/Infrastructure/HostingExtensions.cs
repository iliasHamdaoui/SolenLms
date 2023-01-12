using Imanys.SolenLms.IdentityProvider.Infrastructure.DI;
using Microsoft.AspNetCore.Builder;
using Serilog;
using static Imanys.SolenLms.IdentityProvider.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseExceptionHandler("/error");

        app.UseApplicationSwaggerUI();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors("Open");

        app.MapRazorPages();

        app.MapControllers().RequireAuthorization(AuthenticatedUserPolicy);



        return app;
    }
}
