using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Instructors;

internal sealed class UserDeletedHandler : INotificationHandler<UserDeleted>
{
    private readonly CourseManagementDbContext _dbContext;
    private readonly ILogger<UserDeletedHandler> _logger;

    public UserDeletedHandler(CourseManagementDbContext dbContext, ILogger<UserDeletedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(UserDeleted @event, CancellationToken cancellationToken)
    {
        try
        {
            int count = await DeleteInstructorFromRepository(@event.UserId, cancellationToken);

            _logger.LogInformation("Instructor deleted. UserId:{UserId}, count:{count}", @event.UserId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while deleting instructor. UserId:{UserId}, message:{message}",
                @event.UserId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteInstructorFromRepository(string instructorId, CancellationToken cancellationToken)
    {
        return await _dbContext.Instructors
            .Where(x => x.Id == instructorId)
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion

}