namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerProgressAggregate;


public sealed class LearnerCourseProgress : IAggregateRoot
{
    public string LearnerId { get; init; }
    public int CourseId { get; init; }
    public float Progress { get; private set; }
    public DateTime LastAccessTime { get; private set; }


    public LearnerCourseProgress(string learnerId, int courseId)
    {
        ArgumentNullException.ThrowIfNull(learnerId, nameof(learnerId));

        LearnerId = learnerId;
        CourseId = courseId;
    }

    public void UpdateProgress(float progress, DateTime lastAccessTime)
    {
        Progress = progress;
        LastAccessTime = lastAccessTime;
    }
}
