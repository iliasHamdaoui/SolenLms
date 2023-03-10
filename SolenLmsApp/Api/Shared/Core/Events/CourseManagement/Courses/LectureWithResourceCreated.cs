using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.Shared.Core.Events;

#nullable disable

public sealed record LectureWithResourceCreated : BaseIntegrationEvent
{
    public string OrganizationId { get; init; }
    public string CourseId { get; init; }
    public string ModuleId { get; init; }
    public string LectureId { get; init; }
    public MediaType MediaType { get; init; }
    public override string EventType => nameof(LectureWithResourceCreated);
}
