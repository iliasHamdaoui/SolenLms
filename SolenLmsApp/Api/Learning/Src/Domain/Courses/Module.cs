using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.Courses;

public sealed class Module : BaseEntity<string>
{
    private readonly List<Lecture> _lectures = new();

    private Module(string id, string title, int order, int duration)
    {
        Id = id;
        Title = title;
        Order = order;
        Duration = duration;
    }

    public Module(Course course, string id, string title, int order, int duration) : this(id, title, order, duration)
    {
        ArgumentNullException.ThrowIfNull(course, nameof(course));

        Course = course;
    }

    public Course Course { get; init; } = default!;
    public string Title { get; private set; }
    public int Order { get; private set; }
    public int Duration { get; private set; }
    public IEnumerable<Lecture> Lectures => _lectures.AsReadOnly();

    public void UpdateTitle(string title)
    {
        Title = title;
    }

    public void UpdateOrder(int order)
    {
        Order = order;
    }

    public void UpdateDuration(int duration)
    {
        Duration = duration;
    }

    public Lecture AddOrUpdateLecture(string id, string title, LectureType type, int order, int duration,
        string? resourceId)
    {
        var lecture = _lectures.FirstOrDefault(x => x.Id == id);
        if (lecture == null)
        {
            lecture = new Lecture(this, id, title, type, order, duration, resourceId);
            _lectures.Add(lecture);
        }
        else
        {
            lecture.UpdateTitle(title);
            lecture.UpdateOrder(order);
            lecture.UpdateDuration(duration);
        }

        return lecture;
    }
}