namespace Imanys.SolenLms.Application.Shared.Infrastructure.Events;

internal sealed class AzureServiceBusEventsSettings
{
    public string TopicName { get; set; } = default!;
    public string SubscriptionName { get; set; } = default!;
}