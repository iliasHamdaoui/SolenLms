using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.DeleteOrganization;
internal sealed class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, RequestResponse>
{
    private readonly IOrganizationService _organizationService;
    private readonly ILogger<DeleteOrganizationCommandHandler> _logger;

    public DeleteOrganizationCommandHandler(IOrganizationService organizationService, ILogger<DeleteOrganizationCommandHandler> logger)
    {
        _organizationService = organizationService;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(DeleteOrganizationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var organizationToDelete = await _organizationService.GetTheCurrentUserOrganization(cancellationToken);

            await _organizationService.DeleteOrganization(organizationToDelete!, cancellationToken);

            _logger.LogWarning("Organization deleted, {command}", command);

            return RequestResponse.Ok("The organization has been deleted.");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured while deleting the organization, {message}, {command}", ex.Message, command);
            return RequestResponse.Error(ResponseError.Unexpected, "Error occured while deleting the organization.");
        }
    }
}
