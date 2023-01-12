using Fluid;
using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text;
using System.Threading.Channels;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;

internal class EmailBackgroundService : BackgroundService, IEmailService
{
    private static readonly FluidParser _parser = new();
    private readonly Channel<EmailMessage> _channel;
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(IOptions<EmailSettings> emailSettingsOptions, ILogger<EmailBackgroundService> logger)
    {
        _channel = Channel.CreateUnbounded<EmailMessage>();
        _emailSettings = emailSettingsOptions.Value;
        _logger = logger;
    }

    public async Task SendUserEmailVerification(string userEmail, string link)
    {
        StringBuilder body = new();
        body.AppendLine("<h2>Hello ! Let's confirm your email address.</h2>");
        body.AppendLine("<br/>");
        body.AppendLine($"To confirm your email address, please <a href={link}>click here</a>.");
        body.AppendLine("<br/> <br/>");
        body.AppendLine("Solen LMS Team.");

        EmailMessage message = new(userEmail, "Welcome to Solen LMS ! Please confirm your email", body.ToString());

        await SendEmail(message);
    }

    public async Task SendUserInvitationToJoinOrganization(string? inviterName, string invitedEmail,
        string organizationName,
        string link)
    {
        StringBuilder emailTemplate = new();
        emailTemplate.AppendLine("Hello,");
        emailTemplate.AppendLine("<br/> <br/>");
        emailTemplate.Append(
            inviterName != null ? "{{ InviterName }} has invited you" : $"You have been invited");

        emailTemplate.AppendLine(
            " to join <strong>{{ OrganizationName }}</strong> on <i>Solen LMS</i>, an online learning platform. <br/>");

        emailTemplate.AppendLine($"Please <a href={link}>click here</a> to complete the sign up process.");
        emailTemplate.AppendLine("<br/> <br/>");
        emailTemplate.AppendLine("Solen LMS Team.");


        if (_parser.TryParse(emailTemplate.ToString(), out IFluidTemplate? template, out string? _))
        {
            var model = new { InviterName = inviterName, OrganizationName = organizationName };

            string? body = await template.RenderAsync(new TemplateContext(model));

            EmailMessage message = new(invitedEmail, "Welcome to Solen LMS!", body);

            await SendEmail(message);
        }
    }

    public async Task SendPasswordResetRequest(string userEmail, string link)
    {
        StringBuilder body = new();
        body.AppendLine($"<h2>You have submitted a password change request</h2> <br/>");
        body.AppendLine(
            $"If it wasn't you please disregard this email and make sure you can still login to your account. If it was you, then confirm the password change <a href={link}>click here</a>.");
        body.AppendLine("<br/><br/>");
        body.AppendLine("Solen LMS Team.");

        EmailMessage message = new(userEmail, "Password Reset Request", body.ToString());

        await SendEmail(message);
    }

    #region private methods

    private async Task SendEmail(EmailMessage email)
    {
        while (await _channel.Writer.WaitToWriteAsync())
        {
            if (_channel.Writer.TryWrite(email)) return;
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (EmailMessage email in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            await SendEmailAsync(email);
        }
    }

    private async Task SendEmailAsync(EmailMessage email)
    {
        SendGridClient client = new(_emailSettings.SendgridApiKey);
        EmailAddress from = new(_emailSettings.From);
        EmailAddress to = new(email.To);
        string plainTextContent = email.Body;
        string htmlContent = email.Body;

        SendGridMessage? msg = MailHelper.CreateSingleEmail(@from, to, email.Subject, plainTextContent, htmlContent);
        try
        {
            Response? response = await client.SendEmailAsync(msg);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("failed to send email {StatusCode}", response.StatusCode);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "SendEmailAsync Failed");
        }
    }


    private record struct EmailMessage(string To, string Subject, string Body);

    #endregion
}