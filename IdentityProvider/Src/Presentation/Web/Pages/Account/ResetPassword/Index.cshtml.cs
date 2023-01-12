using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.ResetPassword;

public class IndexModel : PageModel
{
    private readonly IAccountService _accountService;

    [BindProperty]
    public ResetPasswordInput Input { get; set; }
    public IndexModel(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public IActionResult OnGet(string userEmail, string securityToken)
    {
        Input = new ResetPasswordInput { Email = userEmail, ResetToken = securityToken };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.RestUserPassword(Input.Email, Input.ResetToken, Input.Password);
            if (result.isSuccess)
                return RedirectToPage("PasswordResetSucceded");

            ModelState.AddModelError(string.Empty, result.error);
        }

        return Page();
    }
}
