using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnersProgress;

namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.Learners;
public sealed class Learner : IAggregateRoot
{
    private readonly List<LearnerCourseProgress> _coursesProgress = new();

    public Learner(string id, string organizationId, string givenName, string familyName)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(organizationId, nameof(organizationId));
        ArgumentNullException.ThrowIfNull(givenName, nameof(givenName));
        ArgumentNullException.ThrowIfNull(familyName, nameof(familyName));

        Id = id;
        OrganizationId = organizationId;
        GivenName = givenName;
        FamilyName = familyName;
    }

    public string Id { get; init; }
    public string OrganizationId { get; init; }
    public string GivenName { get; private set; }
    public string FamilyName { get; private set; }
    public IEnumerable<LearnerCourseProgress> CoursesProgress => _coursesProgress.AsReadOnly();
    public string FullName => $"{GivenName} {FamilyName}";

}
