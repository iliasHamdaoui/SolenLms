using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;

public sealed class Lecture : BaseEntity<string>
{
    private Lecture(string id, string title, LectureType type, int order, int duration, string? resourceId)
    {
        Id = id;
        Title = title;
        Type = type;
        Order = order;
        Duration = duration;
        ResourceId = resourceId;
    }

    public Lecture(Module module, string id, string title, LectureType type, int order, int duration,
        string? resourceId) : this(id, title, type, order, duration, resourceId)
    {
        ArgumentNullException.ThrowIfNull(module, nameof(module));

        Module = module;
    }

    public Module Module { get; init; } = default!;
    public string Title { get; private set; }
    public LectureType Type { get; init; }
    public int Order { get; private set; }
    public int Duration { get; private set; }
    public string? ResourceId { get; init; }

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
}