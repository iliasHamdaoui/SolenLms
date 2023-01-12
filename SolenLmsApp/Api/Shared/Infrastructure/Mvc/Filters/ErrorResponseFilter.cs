using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.Mvc.Filters;

internal sealed class ErrorResponseFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {

        if (context.Result is not ObjectResult { Value: RequestResponse { IsSuccess: false } errorResponse })
            return;
        
        ProblemDetails problem = new()
        {
            Type = errorResponse.GetResponseError()?.Type,
            Title = errorResponse.GetResponseError()?.Title,
            Detail = errorResponse.Message,
            Status = errorResponse.GetResponseError()?.Code,
            Instance = context.HttpContext.Request.Path,
            Extensions = {
                ["traceId"] = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier
            }
        };
        context.Result = new ObjectResult(problem);
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }
}
