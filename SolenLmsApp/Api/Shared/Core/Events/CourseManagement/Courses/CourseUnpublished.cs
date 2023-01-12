namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;


public sealed record CourseUnpublished(string CourseId) : BaseIntegratedEvent
{
    public override string EventType => nameof(CourseUnpublished);
}
