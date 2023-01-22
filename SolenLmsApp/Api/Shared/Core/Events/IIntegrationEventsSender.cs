namespace Imanys.SolenLms.Application.Shared.Core.Events;

public interface IIntegrationEventsSender
{
    Task<bool> SendEvent(BaseIntegrationEvent @event, CancellationToken ct = default);
}
