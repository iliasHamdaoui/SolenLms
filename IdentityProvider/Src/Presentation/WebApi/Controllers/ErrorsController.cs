using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Imanys.SolenLms.IdentityProvider.WebApi.Controllers;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ErrorsController : ControllerBase
{
    private readonly IHostEnvironment _env;

    public ErrorsController(IHostEnvironment env)
    {
        _env = env;
    }


    [Route("/error")]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()!.Error;
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = exception.Message;

        switch (exception)
        {
            case ArgumentNullException ex:
                if (ex.Source == "MediatR")
                    statusCode = (int)HttpStatusCode.BadRequest; break;
        }

        if (_env.IsProduction() && statusCode == (int)HttpStatusCode.InternalServerError)
            message = "An error has occured, please try later.";

        HttpContext.Response.ContentType = "application/json";
        return Problem(title: message, statusCode: statusCode, detail: message);
    }
}
