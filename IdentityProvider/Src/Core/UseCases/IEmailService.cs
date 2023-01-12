namespace Imanys.SolenLms.IdentityProvider.Core.UseCases;

public interface IEmailService
{
    //   Task<bool> SendEmail(EmailMessage email);
    Task SendUserEmailVerification(string userEmail, string link);

    Task SendUserInvitationToJoinOrganization(string? inviterName, string invitedEmail, string organizationName,
        string link);

    Task SendPasswordResetRequest(string userEmail, string link);
}