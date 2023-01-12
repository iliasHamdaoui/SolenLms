using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Imanys.SolenLms.Application.Shared.WebApi;

[AllowAnonymous]
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ErrorsController : ControllerBase
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<ErrorsController> _logger;

    public ErrorsController(IHostEnvironment env, ILogger<ErrorsController> logger)
    {
        _env = env;
        _logger = logger;
    }


    [Route("/error")]
    public IActionResult Error()
    {
        Exception exception = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;
       
        _logger.LogCritical(exception, "Unhandled error occured, {message}", exception.Message);
        
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string message = exception.Message;

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
