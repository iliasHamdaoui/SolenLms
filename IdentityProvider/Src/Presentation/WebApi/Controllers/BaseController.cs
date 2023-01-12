
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.IdentityProvider.WebApi.Controllers;


#nullable disable

[ApiController]
[Produces("application/json"), Consumes("application/json")]
[RoutePrefix("idp-api")]
public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext?.RequestServices.GetService<IMediator>();
}
