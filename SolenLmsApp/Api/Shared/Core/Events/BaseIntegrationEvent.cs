using MediatR;

namespace Imanys.SolenLms.Application.Shared.Core.Events;

public abstract record BaseIntegrationEvent : INotification
{
    public abstract string EventType { get; }
}
