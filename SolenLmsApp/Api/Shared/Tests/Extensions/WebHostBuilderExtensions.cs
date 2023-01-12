using Imanys.SolenLms.Application.Shared.Tests.Helpers;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;
using Microsoft.AspNetCore.TestHost;

namespace Imanys.SolenLms.Application.Shared.Tests.Extensions;

public static class WebHostBuilderExtensions
{
    public static IWebHostBuilder WithUser(this IWebHostBuilder builder, TestUser user)
    {
        return builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication("TestAuthentication")
                .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    "TestAuthentication", options =>
                    {
                        options.Role = user.Role;
                        options.OrganizationId = user.OrganizationId;
                        options.Id = user.Id;
                        options.Email = user.Email;
                        options.GivenName = user.GivenName;
                        options.FamilyName = user.FamilyName;
                    });
        });
    }
}
