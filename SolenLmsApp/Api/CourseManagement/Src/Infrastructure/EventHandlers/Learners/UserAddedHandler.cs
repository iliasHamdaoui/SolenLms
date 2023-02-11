using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Learners;
using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.EventHandlers.Learners;

internal sealed class UserAddedHandler : INotificationHandler<UserAdded>
{
    private readonly CourseManagementDbContext _dbContext;
    private readonly ILogger<UserAddedHandler> _logger;

    public UserAddedHandler(CourseManagementDbContext dbContext, ILogger<UserAddedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(UserAdded @event, CancellationToken cancellationToken)
    {
        try
        {
            if (await LearnerAlreadyExists(@event.UserId))
                return;

            Learner learner = new(@event.UserId, @event.OrganizationId, @event.GivenFamily, @event.FamilyFamily);

            await AddLearnerToRepository(learner, cancellationToken);

            _logger.LogInformation("Learner added. OrganizationId:{OrganizationId}, UserId:{UserId}",
                @event.OrganizationId, @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while adding new learner. OrganizationId:{OrganizationId}, UserId:{UserId}, message:{message}",
                @event.OrganizationId, @event.UserId, ex.Message);
        }
    }

    #region private methods

    private Task<bool> LearnerAlreadyExists(string learnerId)
    {
        return _dbContext.Learners.IgnoreQueryFilters().AnyAsync(x => x.Id == learnerId);
    }

    private async Task AddLearnerToRepository(Learner learner, CancellationToken cancellationToken)
    {
        _dbContext.Learners.Add(learner);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}