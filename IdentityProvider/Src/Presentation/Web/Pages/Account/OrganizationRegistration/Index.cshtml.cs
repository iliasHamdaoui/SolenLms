using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.OrganizationRegistration;

[SecurityHeaders]
public class Index : PageModel
{
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;

    public Index(IAccountService accountService, IEmailService emailService)
    {
        _accountService = accountService;
        _emailService = emailService;
    }

    [BindProperty]
    public InputModel Input { get; set; }


    public IActionResult OnGet(string returnUrl)
    {
        Input = new InputModel
        {
            ReturnUrl = returnUrl
        };

        return Page();
    }
    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {

            var (isSucess, admin, error) = await _accountService.RegisterOrganization(Input.OrganizationName, Input.GivenName, Input.FamilyName, Input.Email, Input.Password);

            if (!isSucess)
            {
                ModelState.AddModelError(string.Empty, error);
                return Page();
            }

            return RedirectToPage("/Account/UserActivation/Index", new { userEmail = admin.Email });
        }


        return Page();
    }
}
