namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

public abstract class TestUser
{
    public string OrganizationId { get; set; }
    public string Id { get; set; }
    public abstract string Role { get; init; }
    public string Email { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }

}
