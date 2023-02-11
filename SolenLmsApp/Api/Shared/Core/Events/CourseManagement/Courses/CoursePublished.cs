using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.Shared.Core.Events;

public sealed record CoursePublished(PublishedCourse PublishedCourse) : BaseIntegrationEvent
{
    public override string EventType => nameof(CoursePublished);
}

public readonly record struct PublishedCourse
{
    public required string OrganizationId { get; init; }
    public required string CourseId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? InstructorId { get; init; }
    public required int Duration { get; init; }
    public required DateTime PublicationDate { get; init; }
    public required IEnumerable<PublishedCourseModule> Modules { get; init; }
    public required IEnumerable<int> CategoriesId { get; init; }
}

public readonly record struct PublishedCourseLecture
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required LectureType LectureType { get; init; }
    public required int Duration { get; init; }
    public required int Order { get; init; }
    public string? ResourceId { get; init; }
}

public readonly record struct PublishedCourseModule
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required int Duration { get; init; }
    public required int Order { get; init; }
    public required IEnumerable<PublishedCourseLecture> Lectures { get; init; }
}