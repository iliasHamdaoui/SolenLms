namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;

public sealed record CategoryDeleted(int CategoryId) : BaseIntegratedEvent
{
    public override string EventType => nameof(CategoryDeleted);
}