namespace Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Categories;


public sealed record CategoryUpdated(int CategoryId, string CategoryName) : BaseIntegrationEvent
{
    public override string EventType => nameof(CategoryUpdated);
}