namespace Imanys.SolenLms.Application.Shared.Core.Events;

public interface IIntegratedEventsSender
{
    Task<bool> SendEvent(BaseIntegratedEvent @event, CancellationToken ct = default);
}
