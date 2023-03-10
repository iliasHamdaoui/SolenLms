namespace Imanys.SolenLms.Application.Shared.Core.Events;

public sealed record LectureResourceContentUpdated : BaseIntegrationEvent
{
    public required string ResourceId { get; set; }
    public int Duration { get; set; }
    public override string EventType => nameof(LectureResourceContentUpdated);
}
