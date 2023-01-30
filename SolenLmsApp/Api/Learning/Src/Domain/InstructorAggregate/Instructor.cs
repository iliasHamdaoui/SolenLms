using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.InstructorAggregate;

public sealed class Instructor : IAggregateRoot
{
    public Instructor(string id, string organizationId, string givenName, string familyName)
    {
        Id = id;
        OrganizationId = organizationId;
        GivenName = givenName;
        FamilyName = familyName;
    }

    public string Id { get; init; }
    public string OrganizationId { get; init; }
    public string GivenName { get; private set; }
    public string FamilyName { get; private set; }
    public string FullName => $"{FamilyName} {GivenName}";

    public void UpdateName(string givenName, string familyName)
    {
        GivenName = givenName;
        FamilyName = familyName;
    }
}