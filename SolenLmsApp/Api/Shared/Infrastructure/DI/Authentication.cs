using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.DI;

internal static class Authentication
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        //if (environment.IsDevelopment())
        //{
        //    services.AddAuthentication("DevAuthentication")
        //         .AddScheme<LocalAuthenticationSchemeOptions, LocalAuthenticationHandler>("DevAuthentication", options => options.Role = "Instructor");
        //}
        //else
        //{
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
              {
                  options.Authority = configuration["oidc:Authority"];

                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateAudience = false,
                      ValidTypes = new[] { "at+jwt" },
                  };
                  options.MapInboundClaims = false;
              });
        //  }

        return services;
    }
}
