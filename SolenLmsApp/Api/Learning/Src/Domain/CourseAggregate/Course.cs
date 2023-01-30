using Imanys.SolenLms.Application.Learning.Core.Domain.BookmarkAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.InstructorAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;

public sealed class Course : IAggregateRoot
{
    private readonly List<CourseCategory.CourseCategory> _categories = new();
    private readonly List<Module> _modules = new();
    private readonly List<BookmarkedCourse> _learnersBookmarks = new();
    private readonly List<LearnerCourseProgress> _learnersProgress = new();

    public Course(string id, string organizationId)
    {
        Id = id;
        OrganizationId = organizationId;
    }

    public string Id { get; init; }
    public string OrganizationId { get; init; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public DateTime PublicationDate { get; private set; }
    public int Duration { get; private set; }
    public bool IsPublished { get; private set; }
    public IEnumerable<Module> Modules => _modules.AsReadOnly();
    public IEnumerable<CourseCategory.CourseCategory> Categories => _categories.AsReadOnly();
    public IEnumerable<BookmarkedCourse> LearnersBookmarks => _learnersBookmarks.AsReadOnly();
    public IEnumerable<LearnerCourseProgress> LearnersProgress => _learnersProgress.AsReadOnly();
    public Instructor? Instructor { get; private set; }
    public string? InstructorId { get; private set; }

    public void UpdateInstructor(string? instructorId)
    {
        InstructorId = instructorId;
    }


    public Module AddOrUpdateModule(string id, string title, int order, int duration)
    {
        Module? module = _modules.FirstOrDefault(m => m.Id == id);
        if (module == null)
        {
            module = new Module(this, id, title, order, duration);
            _modules.Add(module);
        }
        else
        {
            module.UpdateTitle(title);
            module.UpdateOrder(order);
            module.UpdateDuration(duration);
        }

        return module;
    }

    public void SetTitle(string title)
    {
        Title = title;
    }

    public void SetDescription(string? description)
    {
        Description = description;
    }

    public void SetPublicationDate(DateTime publicationDate)
    {
        PublicationDate = publicationDate;
        IsPublished = true;
    }

    public void SetDuration(int duration)
    {
        Duration = duration;
    }

    public void Unpublished()
    {
        IsPublished = false;
    }

    public void AddCategory(int categoryId)
    {
        if (_categories.Any(x => x.CategoryId == categoryId))
            return;
        
        _categories.Add(new CourseCategory.CourseCategory(Id, categoryId));
    }
    
    public void RemoveCategory(CourseCategory.CourseCategory category)
    {
        if (_categories.All(x => x.CategoryId != category.CategoryId))
            return;

        _categories.Remove(category);
    }
}