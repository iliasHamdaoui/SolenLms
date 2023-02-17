using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("solenLmsProfile", new [] { "organizationId", ClaimTypes.Role })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[]
        {
            new ApiScope("solenLmsApi", "Solen LMS API", new[] {"organizationId", ClaimTypes.Role }),
            new ApiScope(IdentityServerConstants.LocalApi.ScopeName, new[] {ClaimTypes.Role })
        };

    public static IEnumerable<Client> Clients(IConfiguration configuration)
    {
        var clientsUrls = configuration["WebClient:Url"]!.Split(';');
        var redirectUris = clientsUrls.Select(url => $"{url}/signin-oidc").ToList();
        var logoutUris = clientsUrls.Select(url => $"{url}/signout-callback-oidc").ToList();
        var frontChannelLogoutUri = $"{clientsUrls?.First()}/signout-oidc";

        return new[]
        {
            new Client
            {
                ClientId = configuration["WebClient:ClientId"],
                ClientSecrets = { new Secret(configuration["WebClient:ClientSecrets"].Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = redirectUris,
                FrontChannelLogoutUri = frontChannelLogoutUri,
                PostLogoutRedirectUris = logoutUris,

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "solenLmsApi", "solenLmsProfile", IdentityServerConstants.LocalApi.ScopeName },
                AccessTokenLifetime = 300,
                UpdateAccessTokenClaimsOnRefresh = true,
            },
        };
    }
}
