using Imanys.SolenLms.Application.Shared.Core.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


#nullable disable

namespace Imanys.SolenLms.Application.Shared.Infrastructure.RabbitMq;
internal sealed class RabbitMqIntegrationEventListener : BackgroundService
{
    private readonly ILogger<RabbitMqIntegrationEventListener> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchange;
    private readonly IServiceProvider _serviceProvider;
    private string _consumerTag;
    private string _queueName;

    public RabbitMqIntegrationEventListener(IServiceProvider serviceProvider, IOptions<RabbitMqSettings> settings,
    ILogger<RabbitMqIntegrationEventListener> logger)
    {

        try
        {
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                HostName = settings.Value.Hostname,
                Port = settings.Value.Port,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _exchange = settings.Value.Exchange;

            _channel.ExchangeDeclare(_exchange, type: ExchangeType.Fanout);

            _queueName = _channel.QueueDeclare("webapi").QueueName;
            _channel.QueueBind(queue: _queueName, exchange: _exchange, routingKey: "");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Could not connect to the Message Bus: {message}", ex.Message);
        }
        _serviceProvider = serviceProvider;
    }



    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.BasicQos(0, 1, false);

        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += OnEventReceived;

        _consumerTag = _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task OnEventReceived(object model, BasicDeliverEventArgs response)
    {
        try
        {

            var eventName = Encoding.UTF8.GetString((byte[])response.BasicProperties.Headers["EventName"]);

            Type eventType = Type.GetType($"Imanys.SolenLms.Application.Shared.Core.Events.{eventName}, Shared.Core");

            if (eventType is null)
            {
                _logger.LogWarning("Unknown event type, {event}", eventName);
                _channel.BasicAck(response.DeliveryTag, false);
                return;
            }

            var serializedEvent = Encoding.UTF8.GetString(response.Body.ToArray());

            BaseIntegrationEvent createdEvent = (BaseIntegrationEvent)JsonSerializer.Deserialize(serializedEvent, eventType);

            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(createdEvent);

            _channel.BasicAck(response.DeliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occured while handling the event : {message} ", e.Message);
            _channel!.BasicNack(response.DeliveryTag, false, false);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.BasicCancel(_consumerTag);
        _channel?.Close();
        _connection?.Close();

        return base.StopAsync(cancellationToken);
    }
}
