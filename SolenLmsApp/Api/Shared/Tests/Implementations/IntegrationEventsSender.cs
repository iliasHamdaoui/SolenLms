using Imanys.SolenLms.Application.Shared.Core.Events;
using MediatR;

namespace Imanys.SolenLms.Application.Shared.Tests.Implementations;

public sealed class IntegrationEventsSender : IIntegrationEventsSender
{
    private readonly IMediator _mediator;

    public IntegrationEventsSender(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<bool> SendEvent(BaseIntegrationEvent @event, CancellationToken ct = default)
    {
        await _mediator.Publish(@event, ct);

        return true;
    }
}