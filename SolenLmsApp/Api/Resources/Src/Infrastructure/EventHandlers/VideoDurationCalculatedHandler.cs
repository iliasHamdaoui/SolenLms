using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Resources.Infrastructure.VideoDurationCalculator;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.EventHandlers;

internal sealed class VideoDurationCalculatedHandler : INotificationHandler<VideoDurationCalculated>
{
    private readonly ResourcesDbContext _dbContext;
    private readonly IIntegrationEventsSender _eventsSender;
    private readonly IHashids _hashids;

    public VideoDurationCalculatedHandler(ResourcesDbContext dbContext, IIntegrationEventsSender eventsSender,
        IHashids hashids)
    {
        _dbContext = dbContext;
        _eventsSender = eventsSender;
        _hashids = hashids;
    }

    public async Task Handle(VideoDurationCalculated @event, CancellationToken cancellationToken)
    {
        var resource =
            await _dbContext.Resources.AsNoTracking().IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Data == @event.ResourceName, cancellationToken);

        if (resource is null)
            return;

        LectureResourceContentUpdated contentUpdatedEvent = new()
        {
            ResourceId = _hashids.Encode(resource.Id), Duration = @event.Duration
        };

        await _eventsSender.SendEvent(contentUpdatedEvent, cancellationToken);
    }
}