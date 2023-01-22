using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

namespace Imanys.SolenLms.Application.Shared.Tests.Extensions;

public static class WebApplicationFactoryExtensions
{
    public static async Task<HttpClient> CreateClientWithUser(this CustomSharedWebApplicationFactory factory, TestUser user)
    {

        var client = factory.WithWebHostBuilder(builder => builder.WithUser(user)).CreateClient();

        var userCreationEvent = new UserAdded(user.OrganizationId, user.Id)
        {
            GivenFamily = user.GivenName,
            FamilyFamily = user.FamilyName,
            Email = user.Email,
            Roles = new[] { user.Role }
        };

        await factory.IntegrationEventsSender.SendEvent(userCreationEvent);

        return client;
    }

    public static HttpClient CreateAnonymousUserClient(this CustomSharedWebApplicationFactory factory)
    {
        return factory.CreateClient();
    }
}
