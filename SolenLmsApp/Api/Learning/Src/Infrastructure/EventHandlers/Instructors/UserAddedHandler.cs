using Imanys.SolenLms.Application.Learning.Core.Domain.Instructors;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.EventHandlers.Instructors;

internal sealed class UserAddedHandler : INotificationHandler<UserAdded>
{
    private readonly LearningDbContext _dbContext;
    private readonly ILogger<UserAddedHandler> _logger;

    public UserAddedHandler(LearningDbContext dbContext, ILogger<UserAddedHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(UserAdded @event, CancellationToken cancellationToken)
    {
        try
        {
            if (UserIsNotAnAdminNorAnInstructor(@event.Roles))
                return;

            if (await InstructorAlreadyExists(@event.UserId))
                return;

            Instructor instructor = new(@event.UserId, @event.OrganizationId, @event.GivenFamily,
                @event.FamilyFamily);

            await AddInstructorToRepository(instructor, cancellationToken);

            _logger.LogInformation("Instructor added. OrganizationId:{OrganizationId}, UserId:{UserId}",
                @event.OrganizationId, @event.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error occured while adding new instructor. OrganizationId:{OrganizationId}, UserId:{UserId}, message:{message}",
                @event.OrganizationId, @event.UserId, ex.Message);
        }
    }

    #region private methods

    private static bool UserIsNotAnAdminNorAnInstructor(IEnumerable<string> roles)
    {
        return !roles.Any(role => role is UserRole.Admin or UserRole.Instructor);
    }

    private Task<bool> InstructorAlreadyExists(string instructorId)
    {
        return _dbContext.Instructors.IgnoreQueryFilters().AnyAsync(x => x.Id == instructorId);
    }

    private async Task AddInstructorToRepository(Instructor instructor, CancellationToken cancellationToken)
    {
        _dbContext.Instructors.Add(instructor);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    #endregion
}