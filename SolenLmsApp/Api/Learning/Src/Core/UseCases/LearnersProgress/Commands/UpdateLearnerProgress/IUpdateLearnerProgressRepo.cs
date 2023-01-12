using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.LearnersProgress.Commands.UpdateLearnerProgress;
internal interface IUpdateLearnerProgressRepo
{
    void AddLearnerCourseAccess(LearnerCourseAccess learnerCourseAccess);
    Task<LearnerCourseAccess?> GetLearnerCourseAccess(string learnerId, string courseId, string lectureId);
    Task MarkOtherLecturesAsComplete(string learnerId, string courseId, string lectureId);
    Task<LearnerCourseProgress?> GetLearnerCourseProgress(string learnerId, string courseId);
    void AddLearnerCourseProgress(LearnerCourseProgress learnerCourseProgress);
    Task<int> GetCourseCompletedDuration(string learnerId, string courseId);
    Task<Course?> GetCourse(string courseId);

    Task SaveChanges();
}
