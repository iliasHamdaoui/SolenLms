using Imanys.SolenLms.Application.Shared.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.Middlewares;

internal sealed class UserScopeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserScopeMiddleware> _logger;

    public UserScopeMiddleware(RequestDelegate next, ILogger<UserScopeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        if (currentUser.IsLoggedIn)
        {
            using (_logger.BeginScope("Organization:{Organization}, SubjectId:{subject}", currentUser.OrganizationId,
                       currentUser.UserId))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}