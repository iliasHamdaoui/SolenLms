using MediatR;

namespace Imanys.SolenLms.Application.Shared.Core.Events;

public abstract record BaseIntegratedEvent : INotification
{
    public abstract string EventType { get; }
}
