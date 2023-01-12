namespace Imanys.SolenLms.Application.Shared.Core;

public interface ICurrentUser
{
    bool IsLoggedIn { get; }
    public string OrganizationId { get; }
    public string UserId { get; }
}
