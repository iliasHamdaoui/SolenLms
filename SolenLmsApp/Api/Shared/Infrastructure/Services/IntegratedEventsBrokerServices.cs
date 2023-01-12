using Imanys.SolenLms.Application.Shared.Core.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.Services;

internal sealed class IntegratedEventsBrokerServices : BackgroundService, IIntegratedEventsSender
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegratedEventsBrokerServices> _logger;
    private readonly Channel<BaseIntegratedEvent> _channel;

    public IntegratedEventsBrokerServices(IServiceProvider serviceProvider, ILogger<IntegratedEventsBrokerServices> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _channel = Channel.CreateUnbounded<BaseIntegratedEvent>();
    }

    public async Task<bool> SendEvent(BaseIntegratedEvent @event, CancellationToken ct = default)
    {
        while (await _channel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
        {
            if (_channel.Writer.TryWrite(@event))
                return true;
        }

        return false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {

                _logger.LogInformation("Handling the event, {event}", @event);

                using var scope = _serviceProvider.CreateScope();

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Publish(@event, stoppingToken);

                _logger.LogInformation("The event has been treated, {event}", @event);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while handling the event, {event}, {message}", @event, ex.Message);
            }
        }
    }
}
