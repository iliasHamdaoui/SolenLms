using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.AddUser;

internal sealed class AddUserCommandHandler : IRequestHandler<AddUserCommand, RequestResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly IAccountService _accountService;
    private readonly ILogger<AddUserCommandHandler> _logger;

    public AddUserCommandHandler(ICurrentUser currentUser, IAccountService accountService,
        ILogger<AddUserCommandHandler> logger)
    {
        _currentUser = currentUser;
        _accountService = accountService;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        try
        {
            (bool isSuccess, string? error) = await _accountService.AddUser(_currentUser.UserId, command.GivenName,
                command.FamilyName, command.Email, command.Roles);

            if (!isSuccess)
                return RequestResponse.Error(ResponseError.Unprocessable, error);

            _logger.LogInformation("User added (invited), {command}", command);

            return RequestResponse.Ok("The user has been invited.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while adding the user, {message}, {command}", ex.Message, command);
            return RequestResponse.Error(ResponseError.Unexpected, "Error occured while inviting the user.");
        }
    }
}