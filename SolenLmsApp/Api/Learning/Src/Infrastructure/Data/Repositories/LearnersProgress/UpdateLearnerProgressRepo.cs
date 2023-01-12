using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;
using Imanys.SolenLms.Application.Learning.Core.UseCases.LearnersProgress.Commands.UpdateLearnerProgress;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Repositories.LearnersProgress;
internal sealed class UpdateLearnerProgressRepo : IUpdateLearnerProgressRepo
{
    private readonly LearningDbContext _dbContext;

    public UpdateLearnerProgressRepo(LearningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddLearnerCourseAccess(LearnerCourseAccess learnerCourseAccess)
    {
        _dbContext.LearnerCourseAccess.Add(learnerCourseAccess);
    }

    public void AddLearnerCourseProgress(LearnerCourseProgress learnerCourseProgress)
    {
        _dbContext.LearnerCourseProgress.Add(learnerCourseProgress);
    }

    public Task<Course?> GetCourse(string courseId)
    {
        return _dbContext.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == courseId);
    }

    public Task<LearnerCourseAccess?> GetLearnerCourseAccess(string learnerId, string courseId, string lectureId)
    {
        return _dbContext.LearnerCourseAccess.FirstOrDefaultAsync(x => x.LearnerId == learnerId && x.CourseId == courseId && x.LectureId == lectureId);
    }

    public Task<LearnerCourseProgress?> GetLearnerCourseProgress(string learnerId, string courseId)
    {
        return _dbContext.LearnerCourseProgress.FirstOrDefaultAsync(x => x.LearnerId == learnerId && x.CourseId == courseId);
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Task MarkOtherLecturesAsComplete(string learnerId, string courseId, string lectureId)
    {
        return _dbContext.LearnerCourseAccess
                        .Where(x => x.LearnerId == learnerId && x.CourseId == courseId && x.LectureId != lectureId)
                        .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsCompleted, true));
    }

    public Task<int> GetCourseCompletedDuration(string learnerId, string courseId)
    {
        return _dbContext.LearnerCourseAccess.Where(x => x.LearnerId == learnerId && x.CourseId == courseId && x.IsCompleted).Include(x => x.Lecture).SumAsync(x => x.Lecture.Duration);
    }
}
