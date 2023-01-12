using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;

#nullable disable

public sealed record LectureWithResourceCreated : BaseIntegratedEvent
{
    public string OrganizationId { get; init; }
    public string CourseId { get; init; }
    public string ModuleId { get; init; }
    public string LectureId { get; init; }
    public MediaType MediaType { get; init; }
    public override string EventType => nameof(LectureWithResourceCreated);
}
