namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.Instructors;

public sealed class Instructor : IAggregateRoot
{
    public Instructor(string id, string organizationId, string email, string givenName, string familyName)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(organizationId, nameof(organizationId));
        ArgumentNullException.ThrowIfNull(email, nameof(email));
        ArgumentNullException.ThrowIfNull(givenName, nameof(givenName));
        ArgumentNullException.ThrowIfNull(familyName, nameof(familyName));

        Id = id;
        OrganizationId = organizationId;
        Email = email;
        GivenName = givenName;
        FamilyName = familyName;
    }

    public string Id { get; init; }
    public string OrganizationId { get; init; }
    public string Email { get; private set; }
    public string GivenName { get; private set; }
    public string FamilyName { get; private set; }
    public string FullName => $"{FamilyName} {GivenName}";

}
