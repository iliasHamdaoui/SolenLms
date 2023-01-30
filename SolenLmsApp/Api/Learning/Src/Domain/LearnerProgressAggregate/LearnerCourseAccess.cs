using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerAggregate;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;

public sealed class LearnerCourseAccess : IAggregateRoot
{
    public string LearnerId { get; init; }
    public Learner Learner { get; init; } = default!;
    public string LectureId { get; init; }
    public Lecture Lecture { get; init; } = default!;
    public string CourseId { get; init; }
    public DateTime AccessTime { get; private set; }
    public bool IsCompleted { get; private set; }


    public LearnerCourseAccess(string learnerId, string lectureId, string courseId)
    {
        ArgumentNullException.ThrowIfNull(learnerId, nameof(learnerId));
        ArgumentNullException.ThrowIfNull(lectureId, nameof(lectureId));
        ArgumentNullException.ThrowIfNull(courseId, nameof(courseId));

        LearnerId = learnerId;
        LectureId = lectureId;
        CourseId = courseId;
    }

    public void UpdateAccessTime(DateTime accessTime)
    {
        AccessTime = accessTime;
    }

    public void SetCompleted()
    {
        IsCompleted = true;
    }
}