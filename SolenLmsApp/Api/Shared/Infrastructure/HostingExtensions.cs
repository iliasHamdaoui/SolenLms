using Imanys.SolenLms.Application.Shared.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Infrastructure.DI;
using Imanys.SolenLms.Application.Shared.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace Imanys.SolenLms.Application.Shared.Infrastructure;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole();

        if (bool.Parse(builder.Configuration["ApplicationInsights:Enabled"] ?? string.Empty))
        {
            builder.Services.AddApplicationInsightsTelemetry();
        }
        else
        {
            builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Properties:j}{NewLine}{Exception}{NewLine}")
                .WriteTo.Seq("http://seq")
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(ctx.Configuration));
        }

        builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app, IConfiguration configuration)
    {
        app.UseExceptionHandler("/error");

        var forwardedHeaderOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        forwardedHeaderOptions.KnownNetworks.Clear();
        forwardedHeaderOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardedHeaderOptions);

        app.UseApplicationSwaggerUI();

        app.UseStaticFiles();

        app.UseRouting();

        IOptions<RequestLocalizationOptions>? locOptions =
            app.Services.GetService<IOptions<RequestLocalizationOptions>>();
        if (locOptions != null)
            app.UseRequestLocalization(locOptions.Value);


        app.UseAuthentication();
        app.UseMiddleware<UserScopeMiddleware>();
        app.UseAuthorization();

        app.UseCors("Open");

        app.MapControllers().RequireAuthorization();

        if (configuration["applyMigration"] != null)
        {
            app.MigrateDatabase();
        }


        return app;
    }
}