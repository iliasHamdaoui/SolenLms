namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;

public sealed record ModuleDeleted : BaseIntegrationEvent
{
    public required string OrganizationId { get; init; }
    public required string CourseId { get; init; }
    public required string ModuleId { get; init; }
    public override string EventType => nameof(ModuleDeleted);
}
