namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;

public sealed record CourseDeleted : BaseIntegratedEvent
{
    public required string OrganizationId { get; init; }
    public required string CourseId { get; init; }
    public override string EventType => nameof(CourseDeleted);
}