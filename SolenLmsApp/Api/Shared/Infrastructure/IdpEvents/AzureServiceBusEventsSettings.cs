namespace Imanys.SolenLms.Application.Shared.Infrastructure.IdpEvents;

internal sealed class AzureServiceBusEventsSettings
{
    public string TopicName { get; set; } = default!;
    public string SubscriptionName { get; set; } = default!;
}