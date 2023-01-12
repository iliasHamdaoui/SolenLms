using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;

internal class IntegratedEventsBrokerService : BackgroundService, IIntegratedEventsSender
{
    private readonly Channel<BaseIntegratedEvent> _channel;
    private readonly ServiceBusSender _serviceBusSender;
    private readonly ILogger<IntegratedEventsBrokerService> _logger;

    public IntegratedEventsBrokerService(ServiceBusClient serviceBusClient,
        IOptions<AzureServiceBusSettings> settings, ILogger<IntegratedEventsBrokerService> logger)
    {
        _channel = Channel.CreateUnbounded<BaseIntegratedEvent>();
        _serviceBusSender = serviceBusClient.CreateSender(settings.Value.IdpQueueName);
        _logger = logger;
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
                var message = new ServiceBusMessage(JsonConvert.SerializeObject(@event));
                message.ApplicationProperties["eventType"] = @event.EventType;

                await _serviceBusSender.SendMessageAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ocurrend while sending message, {event}", @event);
            }

        }
    }
}
