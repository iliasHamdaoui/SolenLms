using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerAggregate;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerProgressAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress;

internal static class LearnerExtension
{
    public static LearnerForGetCourseLearnersQuery ToLearnerQuery(this Learner learner)
    {
        LearnerCourseProgress? courseProgress = learner.CoursesProgress.FirstOrDefault();

        return new LearnerForGetCourseLearnersQuery
        {
            Name = learner.FullName,
            Progress = courseProgress?.Progress ?? 0,
            LastAccessTime = courseProgress?.LastAccessTime
        };
    }
}