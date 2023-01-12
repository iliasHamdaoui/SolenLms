using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.UpdateOrganization;
internal sealed class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, RequestResponse>
{
    private readonly IOrganizationService _organizationService;
    private readonly ILogger<UpdateOrganizationCommandHandler> _logger;

    public UpdateOrganizationCommandHandler(IOrganizationService organizationService, ILogger<UpdateOrganizationCommandHandler> logger)
    {
        _organizationService = organizationService;
        _logger = logger;
    }
    public async Task<RequestResponse> Handle(UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var organization = await _organizationService.GetTheCurrentUserOrganization(cancellationToken);

            organization!.UpdateName(command.OrganizationName);

            await _organizationService.UpdateOrganization(organization, cancellationToken);

            _logger.LogInformation("Organization updated, {organizationId}, {command}", organization.Id, command);

            return RequestResponse.Ok("The organization has been updated.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while updating the organization, {message}, {command}", ex.Message, command);
            return RequestResponse.Error(ResponseError.Unexpected, "Error occured while updating the organization.");
        }
    }
}
