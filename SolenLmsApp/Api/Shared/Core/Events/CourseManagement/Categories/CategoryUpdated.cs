namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;


public sealed record CategoryUpdated(int CategoryId, string CategoryName) : BaseIntegratedEvent
{
    public override string EventType => nameof(CategoryUpdated);
}