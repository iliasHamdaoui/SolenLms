namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress;
public sealed record GetLearnersProgressQueryResult(IEnumerable<LearnerForGetCourseLearnersQuery> Learners);

public sealed record LearnerForGetCourseLearnersQuery
{
    public string Name { get; set; } = default!;
    public float Progress { get; set; }
    public DateTime? LastAccessTime { get; set; }
}