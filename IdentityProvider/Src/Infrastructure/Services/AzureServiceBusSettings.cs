namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
internal sealed class AzureServiceBusSettings
{
    public string ConnectionString { get; set; } = default!;
    public string IdpQueueName { get; set; } = default!;
}
