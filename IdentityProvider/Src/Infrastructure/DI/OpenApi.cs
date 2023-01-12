using Imanys.SolenLms.IdentityProvider.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZymLabs.NSwag.FluentValidation;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class OpenApi
{
    internal static IServiceCollection AddOpenApi(this IServiceCollection services)
    {

        services.AddOpenApiDocument((settings, serviceProvider) =>
        {
            settings.DocumentName = OpenApiConstants.IdentityProviderOpenApiSpecification;
            settings.ApiGroupNames = new[] { OpenApiConstants.IdentityProviderGroupName };
            settings.Title = "Solen LMS Identity Provider API";
            settings.Description = "Solen LMS API created by IMANYS CONSULTING";
            settings.Version = "1";
            settings.UseXmlDocumentation = true;
            var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();

            // Add the fluent validations schema processor
            settings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
        });


        // Add the FluentValidationSchemaProcessor as a scoped service
        services.AddScoped(provider =>
        {
            var validationRules = provider.GetService<IEnumerable<FluentValidationRule>>();
            var loggerFactory = provider.GetService<ILoggerFactory>();

            return new FluentValidationSchemaProcessor(provider, validationRules, loggerFactory);
        });

        return services;
    }
}

public static class SwaggerExtensions
{
    public static void UseApplicationSwaggerUI(this WebApplication app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi3();
    }
}
