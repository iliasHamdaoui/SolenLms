using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.RegenerateRegistrationCode;
internal class RegenerateRegistrationCodeCommandHandler : IRequestHandler<RegenerateRegistrationCodeCommand, RequestResponse>
{
    private readonly IAccountService _accountService;
    private readonly ILogger<RegenerateRegistrationCodeCommandHandler> _logger;

    public RegenerateRegistrationCodeCommandHandler(IAccountService accountService, ILogger<RegenerateRegistrationCodeCommandHandler> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }
    public async Task<RequestResponse> Handle(RegenerateRegistrationCodeCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var (isSucess, error) = await _accountService.RegenerateUserRegistrationToken(command.Email);
            if (!isSucess)
                return RequestResponse.Error(ResponseError.Unprocessable, error);

            _logger.LogInformation("Registration code generated, {command}", command);

            return RequestResponse.Ok("The code has been regenerated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while regenerating the registration code, {message}, {command}", ex.Message, command);
            return RequestResponse.Error(ResponseError.Unexpected, "Error occured while regenerating the registration code.");
        }
    }
}
