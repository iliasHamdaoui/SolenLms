using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

public sealed class Lecture : BaseEntity
{
    public Lecture(int moduleId, string title, LectureType type, int order)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        ModuleId = moduleId;
        Title = title;
        Type = type;
        Order = order;
    }

    public int ModuleId { get; init; }
    public string Title { get; private set; }
    public LectureType Type { get; init; }
    public int Order { get; private set; }
    public int Duration { get; private set; }
    public string? ResourceId { get; private set; }

    public void UpdateTitle(string title)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));

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

    public void SetResourceId(string resourceId)
    {
        ResourceId = resourceId;
    }
}
