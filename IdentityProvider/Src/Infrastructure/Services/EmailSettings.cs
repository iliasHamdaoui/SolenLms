namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
internal sealed class EmailSettings
{
    public string SendgridApiKey { get; set; } = default!;
    public string From { get; set; } = default!;
    public bool? UseSmtp { get; set; }
    public string SmtpServer { get; set; } = default!;
    public int SmtpPort { get; set; } = default!;
}
