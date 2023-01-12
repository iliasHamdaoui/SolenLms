namespace Imanys.SolenLms.Application.Shared.Core.Events.Resources;

#nullable disable

public sealed record LectureResourceCreated : BaseIntegratedEvent
{
    public string LectureId { get; set; }
    public string ResourceId { get; set; }

    public override string EventType => nameof(LectureResourceCreated);
}
