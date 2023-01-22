namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;

public sealed record CategoryDeleted(int CategoryId) : BaseIntegrationEvent
{
    public override string EventType => nameof(CategoryDeleted);
}