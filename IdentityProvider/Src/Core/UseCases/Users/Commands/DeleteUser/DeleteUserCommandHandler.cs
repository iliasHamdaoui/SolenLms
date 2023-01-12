using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.DeleteUser;
internal sealed class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, RequestResponse>
{
    private readonly IAccountService _accountService;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(IAccountService accountService, ICurrentUser currentUser, ILogger<DeleteUserCommandHandler> logger)
    {
        _accountService = accountService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var userToDelete = await _accountService.GetUser(command.UserEmail);
            if (userToDelete == null || userToDelete.OrganizationId != _currentUser.OrganizationId)
                return RequestResponse.Error(ResponseError.Unprocessable, "The user does not exist.");

            if (userToDelete.Id == _currentUser.UserId)
                return RequestResponse.Error(ResponseError.Unprocessable, "You can not delete yourself. Consider deleting the organization instead.");

            var (isSucess, error) = await _accountService.DeleteUser(userToDelete);
            if (!isSucess)
                return RequestResponse.Error(ResponseError.Unprocessable, error);

            _logger.LogInformation("User deleted, {command}", command);

            return RequestResponse.Ok("The user has been deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while deleting the user, {message}, {command}", ex.Message, command);
            return RequestResponse.Error(ResponseError.Unexpected, "Error occured while deleting the user.");
        }
    }
}
