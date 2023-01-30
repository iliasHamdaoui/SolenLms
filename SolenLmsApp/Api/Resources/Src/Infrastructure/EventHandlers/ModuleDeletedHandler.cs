using Imanys.SolenLms.Application.Resources.Features;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.EventHandlers;

internal sealed class ModuleDeletedHandler : INotificationHandler<ModuleDeleted>
{
    private readonly ResourcesDbContext _dbContext;
    private readonly IMediaManager _mediaManager;
    private readonly ILogger<ModuleDeletedHandler> _logger;

    public ModuleDeletedHandler(ResourcesDbContext dbContext, IMediaManager mediaManager,
        ILogger<ModuleDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _mediaManager = mediaManager;
        _logger = logger;
    }

    public async Task Handle(ModuleDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteModuleResourcesFromRepository(@event.ModuleId, cancellationToken);

            await _mediaManager.DeleteModuleMedias(@event.OrganizationId, @event.CourseId, @event.ModuleId);

            _logger.LogInformation(
                "Module resources deleted. courseId:{courseId}, moduleId:{moduleId}, count:{count}", @event.CourseId,
                @event.ModuleId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting module resources. courseId:{courseId}, moduleId:{moduleId}, message:{message}",
                @event.CourseId, @event.ModuleId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteModuleResourcesFromRepository(string moduleId, CancellationToken cancellationToken)
    {
        return await _dbContext.Resources
            .IgnoreQueryFilters()
            .Where(x => x.ModuleId == moduleId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}