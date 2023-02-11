namespace Imanys.SolenLms.Application.Shared.Core.Events;

public sealed record CategoryDeleted(int CategoryId) : BaseIntegrationEvent
{
    public override string EventType => nameof(CategoryDeleted);
}