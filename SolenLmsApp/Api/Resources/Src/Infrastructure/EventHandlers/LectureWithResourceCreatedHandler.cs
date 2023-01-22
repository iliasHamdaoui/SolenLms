using HashidsNet;
using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;
using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using Imanys.SolenLms.Application.Shared.Core.Events.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.EventHandlers;

internal sealed class LectureWithResourceCreatedHandler : INotificationHandler<LectureWithResourceCreated>
{
    private readonly ResourcesDbContext _dbContext;
    private readonly IHashids _hashids;
    private readonly IIntegrationEventsSender _eventSender;
    private readonly ILogger<LectureWithResourceCreatedHandler> _logger;

    public LectureWithResourceCreatedHandler(ResourcesDbContext dbContext, IHashids hashids,
        IIntegrationEventsSender eventSender, ILogger<LectureWithResourceCreatedHandler> logger)
    {
        _dbContext = dbContext;
        _hashids = hashids;
        _eventSender = eventSender;
        _logger = logger;
    }

    public async Task Handle(LectureWithResourceCreated @event, CancellationToken cancellationToken)
    {
        try
        {
            LectureResource resourceToAdd = new(@event.OrganizationId, @event.CourseId, @event.ModuleId,
                @event.LectureId, @event.MediaType);

            await AddNewResourceToRepository(resourceToAdd, cancellationToken);

            await SendLectureResourceCreatedEvent(resourceToAdd, cancellationToken);

            _logger.LogInformation("Lecture resource created. lectureId:{lectureId}", @event.LectureId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while creating the resource. lectureId:{lectureId}, message:{message}",
                @event.LectureId, ex.Message);
        }
    }

    #region private methods

    private async Task SendLectureResourceCreatedEvent(LectureResource createdResource,
        CancellationToken cancellationToken)
    {
        LectureResourceCreated resourceCreatedEvent = new()
        {
            LectureId = createdResource.LectureId, ResourceId = _hashids.Encode(createdResource.Id)
        };

        await _eventSender.SendEvent(resourceCreatedEvent, cancellationToken);
    }

    private async Task AddNewResourceToRepository(LectureResource resourceToAdd, CancellationToken cancellationToken)
    {
        _dbContext.Resources.Add(resourceToAdd);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}