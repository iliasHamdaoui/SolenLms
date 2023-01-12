using System.ComponentModel.DataAnnotations;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.UserRegistration;


public sealed class RegistrationInput
{
    [Required]
    [MaxLength(60)]
    [Display(Name = "Your given name")]
    public string GivenName { get; set; }

    [Required]
    [MaxLength(60)]
    [Display(Name = "Your family name")]
    public string FamilyName { get; set; }

    [Required]
    [MaxLength(200)]
    [DataType(DataType.Password)]
    [Display(Name = "Your password")]
    public string Password { get; set; }

    [Required]
    [MaxLength(200)]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm your password")]
    [Compare("Password")]
    public string ConfirmPassword { get; set; }

    [Required]
    public string SecurityCode { get; set; }
    [Required]
    public string Email { get; set; }
}