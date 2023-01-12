using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.DeleteOrganization;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.UpdateOrganization;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganization;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganizationName;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Imanys.SolenLms.IdentityProvider.WebApi.OpenApiConstants;
using static Imanys.SolenLms.IdentityProvider.WebApi.PoliciesConstants;

namespace Imanys.SolenLms.IdentityProvider.WebApi.Controllers.Organizations;

[Route("organizations")]
[ApiExplorerSettings(GroupName = IdentityProviderGroupName)]
public sealed class OrganizationsController : BaseController
{

    /// <summary>
    /// Get the organization info for admin
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = AdminPolicy)]
    [ProducesResponseType(typeof(RequestResponse<GetOrganizationQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<GetOrganizationQueryResult>>> GetOrganization(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetOrganizationQuery(), cancellationToken));
    }

    /// <summary>
    /// Get the organization name
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet("name")]
    [ProducesResponseType(typeof(RequestResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<string>>> GetOrganizationName(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetOrganizationNameQuery(), cancellationToken));
    }
    /// <summary>
    /// update information about the organization by the admin
    /// </summary>
    /// <param name="command">Object containing information about the organization to update.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut]
    [Authorize(Policy = AdminPolicy)]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateOrganization(UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        return Ok(await Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// delete the user organization
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpDelete]
    [Authorize(Policy = AdminPolicy)]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> DeleteOrganization(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new DeleteOrganizationCommand(), cancellationToken));
    }
}
