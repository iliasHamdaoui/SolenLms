using Imanys.SolenLms.Application.Shared.Core.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.RabbitMq;
internal sealed class RabbitMqIntegrationEventBroker : BackgroundService, IIntegrationEventsSender
{
    private readonly Channel<BaseIntegrationEvent> _eventsChannel;
    private readonly ILogger<RabbitMqIntegrationEventBroker> _logger;
    private readonly IConnection? _connection;
    private readonly IModel? _rabbitMqChannel;
    private readonly string? _exchange;

    public RabbitMqIntegrationEventBroker(IOptions<RabbitMqSettings> settings, ILogger<RabbitMqIntegrationEventBroker> logger)
    {
        _eventsChannel = Channel.CreateUnbounded<BaseIntegrationEvent>();
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = settings.Value.Hostname,
                Port = settings.Value.Port
            };

            _connection = factory.CreateConnection();
            _rabbitMqChannel = _connection.CreateModel();
            _exchange = settings.Value.Exchange;

            _rabbitMqChannel.ExchangeDeclare(_exchange, type: ExchangeType.Fanout);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Could not connect to the Message Bus: {message}", ex.Message);
        }
    }

    public async Task<bool> SendEvent(BaseIntegrationEvent @event, CancellationToken ct = default)
    {
        while (await _eventsChannel.Writer.WaitToWriteAsync(ct) && !ct.IsCancellationRequested)
        {
            if (_eventsChannel.Writer.TryWrite(@event))
                return true;
        }

        return false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (BaseIntegrationEvent @event in _eventsChannel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                var props = _rabbitMqChannel!.CreateBasicProperties();
                props.Headers = new Dictionary<string, object>
                {
                    { "EventName", @event.GetType().Name }
                };

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));

                _rabbitMqChannel.BasicPublish(_exchange, "", props, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending message, {event}", @event);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _rabbitMqChannel?.Close();
        _connection?.Close();

        return base.StopAsync(cancellationToken);
    }
}
