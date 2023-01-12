using MediatR;
using Microsoft.Extensions.Logging;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.MediatR.Pipelines;

internal sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("Request:{request}", request))
        {
            return await next();
        }
    }
}