namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
internal sealed class AzureServiceBusSettings
{
    public string ConnectionString { get; set; } = default!;
    public string TopicName { get; set; } = default!;
}
