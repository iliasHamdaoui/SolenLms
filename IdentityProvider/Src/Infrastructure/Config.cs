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

    public static IEnumerable<Client> Clients(IConfiguration configuration) =>
        new[]
        {
            new Client
            {
                ClientId = configuration["WebClient:ClientId"],
                ClientSecrets = { new Secret(configuration["WebClient:ClientSecrets"].Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { $"{configuration["WebClient:Url"]}/signin-oidc" },
                FrontChannelLogoutUri = $"{configuration["WebClient:Url"]}/signout-oidc",
                PostLogoutRedirectUris = { $"{configuration["WebClient:Url"]}/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "solenLmsApi", "solenLmsProfile", IdentityServerConstants.LocalApi.ScopeName },
                AccessTokenLifetime = 300,
                UpdateAccessTokenClaimsOnRefresh = true,
            },
        };
}
