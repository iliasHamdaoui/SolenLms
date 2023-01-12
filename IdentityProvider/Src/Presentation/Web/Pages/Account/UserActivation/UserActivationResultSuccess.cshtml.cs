using Imanys.SolenLms.IdentityProvider.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Account.UserActivation;


public class UserActivationResultModel : PageModel
{
    private readonly SolenLmsWebClientUrl _solenLmsWebClientUrl;

    public UserActivationResultModel(IOptions<SolenLmsWebClientUrl> options)
    {
        _solenLmsWebClientUrl = options.Value;
    }
    public void OnGet()
    {
        ViewData["LoginUrl"] = $"{_solenLmsWebClientUrl.Value}/bff/login";
    }
}
