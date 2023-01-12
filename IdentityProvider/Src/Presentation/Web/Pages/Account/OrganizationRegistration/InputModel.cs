using System.ComponentModel.DataAnnotations;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.OrganizationRegistration;

public sealed class InputModel
{
    [MaxLength(200)]
    [Required]
    [Display(Name = "Given Name")]
    public string GivenName { get; set; }

    [MaxLength(200)]
    [Required]
    [Display(Name = "Family Name")]
    public string FamilyName { get; set; }

    [MaxLength(200)]
    [Required]
    [Display(Name = "Organization name")]
    public string OrganizationName { get; set; }

    [MaxLength(200)]
    [DataType(DataType.Password)]
    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }

    public string ReturnUrl { get; set; }
}
