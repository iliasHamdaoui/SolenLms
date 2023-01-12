using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.UserActivation;

[AllowAnonymous]
public class UserActivationModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly SolenLmsWebClientUrl _solenLmsWebClientUrl;

    public UserActivationModel(IAccountService accountService, IOptions<SolenLmsWebClientUrl> options)
    {
        _accountService = accountService;
        _solenLmsWebClientUrl = options.Value;
    }

    public async Task<IActionResult> OnGet(string userEmail, string securityCode)
    {
        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(securityCode))
            return Redirect(_solenLmsWebClientUrl.Value);

        var isSuccess = await _accountService.ActivateUser(userEmail, securityCode);

        if (isSuccess)
            return RedirectToPage("UserActivationResultSuccess");

        return RedirectToPage("UserActivationResultFailure");
    }
}
