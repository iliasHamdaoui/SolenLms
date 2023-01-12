using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.UserActivation;

public class IndexModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;
    private readonly SolenLmsWebClientUrl _solenLmsWebClientUrl;

    public IndexModel(IAccountService accountService, IEmailService emailService,
        IOptions<SolenLmsWebClientUrl> options)
    {
        _accountService = accountService;
        _emailService = emailService;
        _solenLmsWebClientUrl = options.Value;
    }

    public async Task<IActionResult> OnGet(string userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
            return Redirect(_solenLmsWebClientUrl.Value);

        var securityCode = await _accountService.GenerateSecurityCode(userEmail);
        if (securityCode == null)
            return RedirectToPage("ActivationCodeSent");

        var link = Url.PageLink("/Account/UserActivation/ActiveUser", "Get", new { userEmail, securityCode });

        await _emailService.SendUserEmailVerification(userEmail, link!);

        return RedirectToPage("ActivationCodeSent");
    }
}