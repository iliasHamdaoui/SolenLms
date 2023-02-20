namespace Imanys.SolenLms.Application.Shared.Infrastructure.AzureServiceBus;

internal sealed class AzureServiceBusEventsSettings
{
    public string TopicName { get; set; } = default!;
    public string SubscriptionName { get; set; } = default!;
}