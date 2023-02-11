namespace Imanys.SolenLms.Application.Shared.Core.Events;

public sealed record LectureDeleted : BaseIntegrationEvent
{
    public required string OrganizationId { get; init; }
    public required string CourseId { get; init; }
    public required string ModuleId { get; init; }
    public required string LectureId { get; init; }
    public override string EventType => nameof(LectureDeleted);
}

