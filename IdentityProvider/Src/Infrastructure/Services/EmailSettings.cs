namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
internal sealed class EmailSettings
{
    public string SendgridApiKey { get; set; } = default!;
    public string From { get; set; } = default!;
}
