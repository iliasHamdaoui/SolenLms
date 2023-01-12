using Microsoft.AspNetCore.Identity;

namespace Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;

public sealed class User : IdentityUser
{

    private User(string? givenName, string? familyName)
    {

        GivenName = givenName;
        FamilyName = familyName;
        Active = false;
        EmailConfirmed = false;

    }

    public User(Organization organization, string? givenName, string? familyName) : this(givenName, familyName)
    {
        ArgumentNullException.ThrowIfNull(organization, nameof(organization));

        Organization = organization;
    }

    public string OrganizationId { get; init; } = default!;
    public Organization Organization { get; init; } = default!;
    public string? GivenName { get; private set; }
    public string? FamilyName { get; private set; }
    public bool Active { get; set; }
    public string? SecurityCode { get; private set; }
    public DateTime SecurityCodeExpirationDate { get; private set; }

    public string? FullName => $"{GivenName} {FamilyName}";

    public void UpdateFamilyName(string familyName)
    {
        FamilyName = familyName;
    }

    public void UpdateGivenName(string givenName)
    {
        GivenName = givenName;
    }

    public void Activate()
    {
        EmailConfirmed = true;
        Active = true;
        SecurityCode = null;
    }

    public void SetSecurityCode(string code)
    {
        SecurityCode = code;
        SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);
    }
}
