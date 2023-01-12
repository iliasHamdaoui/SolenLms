using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Imanys.SolenLms.Application.Shared.Tests.Helpers;


public sealed class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string OrganizationId { get; set; }
    public string Role { get; set; }
    public string Id { get; set; }
    public string Email { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
}


public sealed class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
                new Claim("organizationId", Options.OrganizationId),
                new Claim(JwtClaimTypes.Subject, Options.Id),
                new Claim(ClaimTypes.Role, Options.Role),
                new Claim(JwtClaimTypes.Email, Options.Email),
                new Claim(JwtClaimTypes.GivenName, Options.GivenName),
                new Claim(JwtClaimTypes.FamilyName, Options.FamilyName),
            };

        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestAuthentication");

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }
}
