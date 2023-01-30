using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

public sealed class Module : BaseEntity
{
    private readonly List<Lecture> _lectures = new();

    public Module(int courseId, string title, int order)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));

        CourseId = courseId;
        Title = title;
        Order = order;
    }

    public int CourseId { get; init; }
    public string Title { get; private set; }
    public int Order { get; private set; }

    public IEnumerable<Lecture> Lectures => _lectures.AsReadOnly();

    public void UpdateTitle(string title)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));

        Title = title;
    }

    public void UpdateOrder(int order)
    {
        Order = order;
    }


    public Lecture AddLecture(string lectureTitle, LectureType lectureType)
    {
        ArgumentNullException.ThrowIfNull(lectureTitle, nameof(lectureTitle));
        ArgumentNullException.ThrowIfNull(lectureType, nameof(lectureType));

        int nextOrder = _lectures.Any() ? _lectures.Max(x => x.Order) + 1 : 1;

        Lecture lecture = new(Id, lectureTitle, lectureType, nextOrder);

        _lectures.Add(lecture);

        return lecture;
    }

    public void RemoveLecture(Lecture lectureToDelete)
    {
        ArgumentNullException.ThrowIfNull(lectureToDelete, nameof(lectureToDelete));

        if (!_lectures.Contains(lectureToDelete))
            return;
        
        _lectures.Remove(lectureToDelete);

        foreach (Lecture lecture in _lectures.Where(x => x.Order > lectureToDelete.Order))
            lecture.UpdateOrder(lecture.Order - 1);
    }
}