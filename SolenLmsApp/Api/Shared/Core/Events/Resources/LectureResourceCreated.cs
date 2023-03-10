namespace Imanys.SolenLms.Application.Shared.Core.Events;

#nullable disable

public sealed record LectureResourceCreated : BaseIntegrationEvent
{
    public string LectureId { get; set; }
    public string ResourceId { get; set; }

    public override string EventType => nameof(LectureResourceCreated);
}
