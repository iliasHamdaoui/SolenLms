using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Shared.WebApi;

[ApiController]
[Produces("application/json"), Consumes("application/json")]
[RoutePrefix("api")]
public abstract class BaseController : ControllerBase
{
    private IMediator _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext?.RequestServices.GetService<IMediator>();
}
