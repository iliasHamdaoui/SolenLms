using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Mvc.Filters;

internal sealed class ErrorResponseFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult && objectResult.Value is RequestResponse requestResponse && !requestResponse.IsSuccess)
        {
            var problem = new ProblemDetails
            {
                Type = requestResponse.GetResponseError()?.Type,
                Title = requestResponse.GetResponseError()?.Title,
                Detail = requestResponse.Message,
                Status = requestResponse.GetResponseError()?.Code,
                Instance = context.HttpContext.Request.Path,
                Extensions = {
                    ["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
                }
            };
            context.Result = new ObjectResult(problem);
        }
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
}
