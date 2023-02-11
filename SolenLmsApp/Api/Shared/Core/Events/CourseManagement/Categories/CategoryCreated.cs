namespace Imanys.SolenLms.Application.Shared.Core.Events;


public sealed record CategoryCreated(string OrganizationId, int CategoryId, string CategoryName) : BaseIntegrationEvent
{
    public override string EventType => nameof(CategoryCreated);
}
