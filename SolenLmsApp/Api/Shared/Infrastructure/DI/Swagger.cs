using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZymLabs.NSwag.FluentValidation;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Swagger
{
    internal static IServiceCollection AddSwagger(this IServiceCollection services)
    {
      
        services.AddOpenApiDocument((settings, serviceProvider) =>
        {
            settings.DocumentName = OpenApiConstants.CourseManagementOpenApiSpecification;
            settings.ApiGroupNames = new[] { OpenApiConstants.CourseManagementGroupName, OpenApiConstants.CourseManagementLearningGroupName };
            settings.Title = "Solen LMS API for Instructors";
            settings.Description = "Solen LMS API created by IMANYS CONSULTING";
            settings.Version = "1";
            settings.UseXmlDocumentation = true;
            var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();

            // Add the fluent validations schema processor
            settings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
        });

        services.AddOpenApiDocument((settings, serviceProvider) =>
        {
            settings.DocumentName = OpenApiConstants.LearningOpenApiSpecification;
            settings.ApiGroupNames = new[] { OpenApiConstants.LearningGroupName, OpenApiConstants.CourseManagementLearningGroupName };
            settings.Title = "Solen LMS API";
            settings.Description = "Solen LMS API created by IMANYS CONSULTING";
            settings.Version = "1";
            settings.UseXmlDocumentation = true;
            var fluentValidationSchemaProcessor = serviceProvider.CreateScope().ServiceProvider.GetService<FluentValidationSchemaProcessor>();

            // Add the fluent validations schema processor
            settings.SchemaProcessors.Add(fluentValidationSchemaProcessor);
        });

        services.AddOpenApiDocument((settings, serviceProvider) =>
        {
            settings.DocumentName = OpenApiConstants.AdminOpenApiSpecification;
            settings.ApiGroupNames = new[] { OpenApiConstants.AdminGroupName};
            settings.Title = "Solen LMS API for Administrators";
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
