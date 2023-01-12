using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.PasswordResetRequest;

public class IndexModel : PageModel
{
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;

    [BindProperty]
    [Required]
    [MaxLength(200)]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }

    public IndexModel(IAccountService accountService, IEmailService emailService)
    {
        _accountService = accountService;
        _emailService = emailService;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.GeneratePasswordResetToken(Email);
            if (!result.isSuccess)
                return RedirectToPage("PasswordResetRequestSent");

            string link = Url.PageLink("/Account/ResetPassword/Index", "Get",
                new { securityToken = result.token, userEmail = Email });

            await _emailService.SendPasswordResetRequest(Email, link!);

            return RedirectToPage("PasswordResetRequestSent");
        }

        return Page();
    }
}