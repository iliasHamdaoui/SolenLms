namespace Imanys.SolenLms.Application.Shared.Core.Events;


public sealed record CourseUnpublished(string CourseId) : BaseIntegrationEvent
{
    public override string EventType => nameof(CourseUnpublished);
}
