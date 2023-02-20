namespace Imanys.SolenLms.IdentityProvider.Infrastructure.RabbitMq;
internal sealed class RabbitMqSettings
{
    public bool? UseRabbitMq { get; init; }
    public string? Hostname { get; init; }
    public int Port { get; init; }
    public string? Exchange { get; init; }
}
