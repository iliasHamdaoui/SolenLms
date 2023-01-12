using IdentityModel;
using Imanys.SolenLms.Application.Shared.Core;
using Microsoft.AspNetCore.Http;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsLoggedIn => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value
                                is null ? false : true;

    public string OrganizationId => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "organizationId")?.Value
                                ?? throw new ArgumentNullException("OrganizationId");

    public string UserId => _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value
                                ?? throw new ArgumentNullException("UserId");


   
}
