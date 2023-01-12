using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.ResendConfirmationEmail;

public class IndexModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public IActionResult OnGet()
    {
        Input = new InputModel();


        return Page();
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
            return RedirectToPage("/Account/UserActivation/Index", new { userEmail = Input.Email });

        return Page();
    }
}
