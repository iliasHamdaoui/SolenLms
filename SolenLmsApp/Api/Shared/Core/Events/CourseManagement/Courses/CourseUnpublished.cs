namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;


public sealed record CourseUnpublished(string CourseId) : BaseIntegrationEvent
{
    public override string EventType => nameof(CourseUnpublished);
}
