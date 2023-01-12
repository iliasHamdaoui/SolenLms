using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.AddUser;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.DeleteUser;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.RegenerateRegistrationCode;
using Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Imanys.SolenLms.IdentityProvider.WebApi.OpenApiConstants;
using static Imanys.SolenLms.IdentityProvider.WebApi.PoliciesConstants;


namespace Imanys.SolenLms.IdentityProvider.WebApi.Controllers.Users;



[Route("users")]
[ApiExplorerSettings(GroupName = IdentityProviderGroupName)]
[Authorize(Policy = AdminPolicy)]
public sealed class UsersController : BaseController
{

    /// <summary>
    /// Get the organization users
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<GetUsersQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RequestResponse<GetUsersQueryResult>>> GetUsers(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetUsersQuery(), cancellationToken));
    }

    /// <summary>
    /// Add user to the organization
    /// </summary>
    /// <param name="command">Object containing information about the user to add.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> AddUser(AddUserCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        return Ok(await Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Regenerate a user registration coden
    /// </summary>
    /// <param name="command">Object containing information about the user to regenerate code.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut("regenerate-registration-code")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> RegenerateRegistrationCode(RegenerateRegistrationCodeCommand command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        return Ok(await Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Delete a user
    /// </summary>
    /// <param name="userEmail">The email of the user to delete.</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpDelete]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> DeleteUser(string userEmail, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new DeleteUserCommand(userEmail), cancellationToken));
    }
}