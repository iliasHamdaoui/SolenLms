namespace Imanys.SolenLms.Application.Shared.Core.Events;

public sealed record CourseDeleted : BaseIntegrationEvent
{
    public required string OrganizationId { get; init; }
    public required string CourseId { get; init; }
    public override string EventType => nameof(CourseDeleted);
}