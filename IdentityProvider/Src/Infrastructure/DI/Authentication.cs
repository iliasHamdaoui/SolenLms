using Duende.IdentityServer;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.DI;

internal static class Authentication
{
    internal static IServiceCollection AddExternalProviders(this IServiceCollection services)
    {

        services.AddAuthentication()
         .AddGoogle(options =>
         {
             options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

             // register your IdentityServer with Google at https://console.developers.google.com
             // enable the Google+ API
             // set the redirect URI to https://localhost:5001/signin-google
             options.ClientId = "copy client ID from Google here";
             options.ClientSecret = "copy client secret from Google here";
         });

        return services;
    }
}
