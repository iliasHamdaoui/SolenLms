namespace Imanys.SolenLms.Application.Shared.Infrastructure.Services.AzureServiceBus;
internal sealed class AzureServiceBusSettings
{
    public string ConnectionString { get; set; } = default!;
    public string IdpQueueName { get; set; } = default!;
}
