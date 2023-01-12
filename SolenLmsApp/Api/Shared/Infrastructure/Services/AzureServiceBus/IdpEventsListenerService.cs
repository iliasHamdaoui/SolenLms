using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.Application.Shared.Infrastructure.Services.AzureServiceBus;

internal sealed class IdpEventsListenerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IdpEventsCreatorFactory _idpEventsCreatorFactory;
    private readonly ILogger<IdpEventsListenerService> _logger;
    private readonly ServiceBusProcessor _serviceBusProcessor;
    public IdpEventsListenerService(IServiceProvider serviceProvider, ServiceBusClient serviceBusClient,
        IOptions<AzureServiceBusSettings> settings, IdpEventsCreatorFactory idpEventsCreatorFactory,
        ILogger<IdpEventsListenerService> logger)
    {
        _serviceProvider = serviceProvider;
        _serviceBusClient = serviceBusClient;
        _idpEventsCreatorFactory = idpEventsCreatorFactory;
        _logger = logger;
        _serviceBusProcessor = serviceBusClient.CreateProcessor(settings.Value.IdpQueueName, new ServiceBusProcessorOptions { MaxConcurrentCalls = 1, AutoCompleteMessages = false });

    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _serviceBusProcessor.ProcessMessageAsync += MessageHandler;

        _serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

        await _serviceBusProcessor.StartProcessingAsync(stoppingToken);

    }


    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        _logger.LogInformation("Handling the message from IDP, {message}", args.Message.Body.ToString());

        var (isSuccess, createdEvent) = _idpEventsCreatorFactory.GetEvent(args.Message);

        if (!isSuccess)
        {
            _logger.LogWarning("Unknown event type, {message}", args.Message.Body.ToString());
            return;
        }

        try
        {
            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(createdEvent!);

            await args.CompleteMessageAsync(args.Message);

            _logger.LogInformation("Message handled successfully, {message}", args.Message.Body.ToString());

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occured when handling the message, {message}, {exception}", args.Message.Body.ToString(), ex.Message);
        }
    }

    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error occured when receiving a message, {exception}, {args}", args.Exception.Message, args);
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _serviceBusProcessor.StopProcessingAsync(cancellationToken);
        await _serviceBusProcessor.DisposeAsync();
        await _serviceBusClient.DisposeAsync();
    }
}
