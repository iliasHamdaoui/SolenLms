using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Instructors;

internal sealed class UserDeletedHandler : INotificationHandler<UserDeleted>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<UserDeletedHandler> _logger;

    public UserDeletedHandler(LearningDbContext dbContext, ILogger<UserDeletedHandler> logger)
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