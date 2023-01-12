using Imanys.SolenLms.Application.Shared.Core.Events;
using MediatR;

namespace Imanys.SolenLms.Application.Shared.Tests.Implementations;

public sealed class IntegratedEventsSender : IIntegratedEventsSender
{
    private readonly IMediator _mediator;

    public IntegratedEventsSender(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<bool> SendEvent(BaseIntegratedEvent @event, CancellationToken ct = default)
    {
        await _mediator.Publish(@event, ct);

        return true;
    }
}