using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.UserRegistration;

public class IndexModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly SolenLmsWebClientUrl _solenLmsWebClientUrl;

    [BindProperty]
    public RegistrationInput Input { get; set; }
    public IndexModel(IAccountService accountService, IOptions<SolenLmsWebClientUrl> options)
    {
        _accountService = accountService;
        _solenLmsWebClientUrl = options.Value;
    }


    public async Task<IActionResult> OnGet(string userEmail, string securityCode)
    {
        if (!await _accountService.CheckUserSecurityCode(userEmail, securityCode))
            return Redirect(_solenLmsWebClientUrl.Value);

        var user = await _accountService.GetUser(userEmail);

        Input = new RegistrationInput { Email = userEmail, SecurityCode = securityCode, FamilyName = user.FamilyName, GivenName = user.GivenName };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.RegisterUser(Input.Email, Input.SecurityCode, Input.Password, Input.GivenName, Input.FamilyName);
            if (result.isSuccess)
                return RedirectToPage("UserRegistrationResultSuccess");

            ModelState.AddModelError(string.Empty, result.error);
        }

        return Page();
    }
}
