using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Threading.Channels;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;

internal sealed class AzureBusIntegrationEventBroker : BackgroundService, IIntegrationEventsSender
{
    private readonly Channel<BaseIntegrationEvent> _channel;
    private readonly ServiceBusSender _serviceBusSender;
    private readonly ILogger<AzureBusIntegrationEventBroker> _logger;

    public AzureBusIntegrationEventBroker(ServiceBusClient serviceBusClient,
        IOptions<AzureServiceBusSettings> settings, ILogger<AzureBusIntegrationEventBroker> logger)
    {
        _channel = Channel.CreateUnbounded<BaseIntegrationEvent>();
        _serviceBusSender = serviceBusClient.CreateSender(settings.Value.TopicName);
        _logger = logger;
    }

    public async Task<bool> SendEvent(BaseIntegrationEvent @event, CancellationToken ct = default)
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
        await foreach (BaseIntegrationEvent @event in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                ServiceBusMessage message = new(JsonConvert.SerializeObject(@event))
                {
                    ApplicationProperties = { ["eventType"] = @event.EventType }
                };

                await _serviceBusSender.SendMessageAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending message, {event}", @event);
            }
        }
    }
}