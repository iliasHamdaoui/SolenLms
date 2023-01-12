using Imanys.SolenLms.IdentityProvider.Infrastructure.Mvc.Filters;
using Imanys.SolenLms.IdentityProvider.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class Mvc
{
    public static IMvcBuilder AddMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        var mvcBuilder = services.AddControllers(opts =>
        {
            opts.ReturnHttpNotAcceptable = true;

            opts.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status406NotAcceptable));
            opts.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
            opts.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status401Unauthorized));

            opts.Conventions.Add(new RoutePrefixConvention());

            opts.Filters.Add(typeof(ErrorResponseFilter));

            var jsonOutputFormatter = opts.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().FirstOrDefault();

            if (jsonOutputFormatter != null)
            {
                // remove text/json as it isn't the approved media type
                // for working with JSON at API level
                if (jsonOutputFormatter.SupportedMediaTypes.Contains("text/json"))
                {
                    jsonOutputFormatter.SupportedMediaTypes.Remove("text/json");
                }
            }
        }).AddControllersAsServices();

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });



        return mvcBuilder;
    }
}

