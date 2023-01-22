using Azure.Messaging.ServiceBus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.VideoDurationCalculator;

internal sealed class VideoDurationCalculatorListenerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<VideoDurationCalculatorListenerService> _logger;
    private readonly ServiceBusProcessor _serviceBusProcessor;

    public VideoDurationCalculatorListenerService(IServiceProvider serviceProvider, ServiceBusClient serviceBusClient,
        IOptions<VideoDurationCalculatorAzureServiceBusSettings> settings,
        ILogger<VideoDurationCalculatorListenerService> logger)
    {
        _serviceProvider = serviceProvider;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
        _serviceBusProcessor = serviceBusClient.CreateProcessor(settings.Value.VideoDurationCalculatorQueueName,
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
        _logger.LogInformation("Handling the message from Video Duration Calculator, {message}",
            args.Message.Body.ToString());

        try
        {
            var durationCalculated =
                JsonConvert.DeserializeObject<VideoDurationCalculated>(Encoding.UTF8.GetString(args.Message.Body));

            using var scope = _serviceProvider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(durationCalculated!);

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