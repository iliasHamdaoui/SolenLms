using Azure.Messaging.ServiceBus;
using Imanys.SolenLms.Application.Shared.Core.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.AzureServiceBus;

internal sealed class AzureServiceBusEventsListener : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<AzureServiceBusEventsListener> _logger;
    private readonly ServiceBusProcessor _serviceBusProcessor;

    public AzureServiceBusEventsListener(IServiceProvider serviceProvider, ServiceBusClient serviceBusClient,
        IOptions<AzureServiceBusSettings> settings, ILogger<AzureServiceBusEventsListener> logger)
    {
        _serviceProvider = serviceProvider;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
        _serviceBusProcessor = serviceBusClient.CreateProcessor(settings.Value.TopicName, settings.Value.SubscriptionName,
            new ServiceBusProcessorOptions { MaxConcurrentCalls = 1, AutoCompleteMessages = false });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serviceBusProcessor.ProcessMessageAsync += MessageHandler;

        _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

        await _serviceBusProcessor.StartProcessingAsync(stoppingToken);
    }


    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            _logger.LogInformation("Handling the message, {message}", args.Message.Body.ToString());

            string? eventString = args.Message.ApplicationProperties["eventType"].ToString();

            Type? eventType =
                Type.GetType(
                    $"Imanys.SolenLms.Application.Shared.Core.Events.{eventString}, Shared.Core");

            if (eventType is null)
            {
                _logger.LogWarning("Unknown event type, {message}", args.Message.Body.ToString());
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            BaseIntegrationEvent createdEvent =
                (BaseIntegrationEvent)JsonSerializer.Deserialize(Encoding.UTF8.GetString(args.Message.Body),
                    eventType)!;

            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(createdEvent);

            await args.CompleteMessageAsync(args.Message);

            _logger.LogInformation("Message handled successfully, {message}", args.Message.Body.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured when handling the message, {message}, {exception}",
                args.Message.Body.ToString(), ex.Message);
        }
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error occured when receiving a message, {exception}, {args}",
            args.Exception.Message, args);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
        await _serviceBusProcessor.DisposeAsync();
        await _serviceBusClient.DisposeAsync();
    }
}