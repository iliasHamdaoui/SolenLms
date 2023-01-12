using System.ComponentModel.DataAnnotations;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.ResetPassword;

public sealed class ResetPasswordInput
{
    [Required]
    [MaxLength(200)]
    [DataType(DataType.Password)]
    [Display(Name = "Your new password")]
    public string Password { get; set; }

    [Required]
    [MaxLength(200)]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm your new password")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string ResetToken { get; set; }
    [Required]
    public string Email { get; set; }
}
