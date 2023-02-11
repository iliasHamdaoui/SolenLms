using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Learners;

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
            int count = await DeleteLearnerFromRepository(@event.UserId, cancellationToken);

            _logger.LogInformation("Learner deleted. UserId:{UserId}, count:{count}", @event.UserId, count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while deleting learner. UserId:{UserId}, message:{message}",
                @event.UserId, ex.Message);
        }
    }

    #region private methods

    private async Task<int> DeleteLearnerFromRepository(string learnerId, CancellationToken cancellationToken)
    {
        return await _dbContext.Learners
            .Where(x => x.Id == learnerId)
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion
}