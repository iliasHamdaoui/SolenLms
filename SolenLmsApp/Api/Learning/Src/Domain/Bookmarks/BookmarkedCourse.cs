namespace Imanys.SolenLms.Application.Learning.Core.Domain.Bookmarks;

public sealed class BookmarkedCourse : IAggregateRoot
{
    public string LearnerId { get; init; }
    public string CourseId { get; init; }

    public BookmarkedCourse(string learnerId, string courseId)
    {
        ArgumentNullException.ThrowIfNull(learnerId, nameof(learnerId));
        ArgumentNullException.ThrowIfNull(courseId, nameof(courseId));

        LearnerId = learnerId;
        CourseId = courseId;
    }
}