using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Startup.Pages;

public class Signup : PageModel
{
    private readonly string? _idpUrl;

    public Signup(IConfiguration configuration)
    {
        _idpUrl = configuration["oidc:Authority"];
    }

    public IActionResult OnGet()
    {
        return Redirect($"{_idpUrl}/Account/OrganizationRegistration");
    }
}