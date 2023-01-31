using Imanys.SolenLms.Application.Shared.Infrastructure.Mvc.Filters;
using Imanys.SolenLms.Application.Shared.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Mvc
{
    public static IMvcBuilder AddMvc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        var mvcBuilder = services.AddControllers(opts =>
        {
            opts.ReturnHttpNotAcceptable = true;

            opts.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails),StatusCodes.Status406NotAcceptable));
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

        if (!long.TryParse(configuration["FormOptions:MultipartBodyLengthLimit"], out long multipartBodyLengthLimit))
            multipartBodyLengthLimit = 2147483648;

        services.Configure<FormOptions>(x => { x.MultipartBodyLengthLimit = multipartBodyLengthLimit; });
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = multipartBodyLengthLimit;
        });


        return mvcBuilder;
    }
}

