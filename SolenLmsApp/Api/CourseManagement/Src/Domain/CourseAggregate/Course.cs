using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseCategoryAggregate;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.InstructorAggregate;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerProgressAggregate;
using Imanys.SolenLms.Application.Shared.Core.Entities;

namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;

public sealed class Course : BaseAuditEntity, IAggregateRoot
{
    private readonly List<Module> _modules = new();
    private readonly List<CourseCategory> _categories = new();
    private readonly List<LearnerCourseProgress> _learnersProgress = new();

    public Course(string organizationId, string title)
    {
        ArgumentNullException.ThrowIfNull(organizationId, nameof(organizationId));
        ArgumentNullException.ThrowIfNull(title, nameof(title));

        OrganizationId = organizationId;
        Title = title;
    }

    public string OrganizationId { get; init; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? PublicationDate { get; private set; }
    public Instructor? Instructor { get; private set; }
    public string? InstructorId { get; private set; }
    public IEnumerable<Module> Modules => _modules.AsReadOnly();
    public IEnumerable<CourseCategory> Categories => _categories.AsReadOnly();
    public IEnumerable<LearnerCourseProgress> LearnersProgress => _learnersProgress.AsReadOnly();
    public bool IsPublished => PublicationDate is not null;

    public void UpdateTitle(string title)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));

        Title = title;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void UpdateInstructorId(string? instructorId)
    {
        InstructorId = instructorId;
    }

    public void SetPublicationDate(DateTime publicationDate)
    {
        PublicationDate = publicationDate;
    }

    public void ResetPublicationDate()
    {
        PublicationDate = null;
    }

    public Module AddModule(string moduleTitle)
    {
        ArgumentNullException.ThrowIfNull(moduleTitle, nameof(moduleTitle));

        int nextOrder = _modules.Any() ? _modules.Max(x => x.Order) + 1 : 1;

        Module module = new(Id, moduleTitle, nextOrder);

        _modules.Add(module);

        return module;
    }


    public void RemoveModule(Module moduleToDelete)
    {
        ArgumentNullException.ThrowIfNull(moduleToDelete, nameof(moduleToDelete));

        if (!_modules.Contains(moduleToDelete))
            return;
        
        _modules.Remove(moduleToDelete);

        foreach (Module module in _modules.Where(x => x.Order > moduleToDelete.Order))
            module.UpdateOrder(module.Order - 1);
    }
}