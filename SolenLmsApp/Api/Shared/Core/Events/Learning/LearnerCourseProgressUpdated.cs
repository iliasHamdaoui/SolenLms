namespace Imanys.SolenLms.Application.Shared.Core.Events.Learning;


public sealed record LearnerCourseProgressUpdated : BaseIntegratedEvent
{
    public override string EventType => nameof(LearnerCourseProgressUpdated);
    public string LearnerId { get; init; } = default!;
    public string CourseId { get; init; } = default!;
    public float Progress { get; init; }
    public DateTime LastAccessTime { get; init; }
}