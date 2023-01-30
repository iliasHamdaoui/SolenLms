using Imanys.SolenLms.Application.Resources.Features;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.EventHandlers;

internal sealed class OrganizationDeletedHandler : INotificationHandler<OrganizationDeleted>
{
    private readonly ResourcesDbContext _dbContext;
    private readonly IMediaManager _mediaManager;
    private readonly ILogger<OrganizationDeletedHandler> _logger;

    public OrganizationDeletedHandler(ResourcesDbContext dbContext, IMediaManager mediaManager,
        ILogger<OrganizationDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _mediaManager = mediaManager;
        _logger = logger;
    }

    public async Task Handle(OrganizationDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteOrganizationResourcesFromRepository(@event.OrganizationId, cancellationToken);

            await _mediaManager.DeleteOrganizationMedias(@event.OrganizationId);

            _logger.LogInformation(
                "Module resources deleted. OrganizationId:{OrganizationId}, count:{count}", @event.OrganizationId,
                count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while deleting organization resources. OrganizationId:{OrganizationId}, message:{message}",
                @event.OrganizationId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteOrganizationResourcesFromRepository(string organizationId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Resources
            .IgnoreQueryFilters()
            .Where(x => x.OrganizationId == organizationId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}