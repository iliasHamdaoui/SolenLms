using Imanys.SolenLms.IdentityProvider.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.IdentityProvider.Web.Pages.Home;

public class Index : PageModel
{
    private readonly SolenLmsWebClientUrl _solenLmsWebClientUrl;

    public Index(IOptions<SolenLmsWebClientUrl> options)
    {
        _solenLmsWebClientUrl = options.Value;
    }

    public IActionResult OnGet()
    {

        return Redirect(_solenLmsWebClientUrl.Value);
        //Version = typeof(Duende.IdentityServer.Hosting.IdentityServerMiddleware).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+').First();
    }
}