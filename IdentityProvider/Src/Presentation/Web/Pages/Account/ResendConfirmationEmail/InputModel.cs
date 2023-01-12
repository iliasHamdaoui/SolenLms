using System.ComponentModel.DataAnnotations;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.ResendConfirmationEmail;

public sealed class InputModel
{
    [Required]
    [MaxLength(200)]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }
}
