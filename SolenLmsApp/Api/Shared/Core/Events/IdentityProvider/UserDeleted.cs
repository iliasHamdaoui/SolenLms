namespace Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;

public sealed record UserDeleted(string UserId) : BaseIntegrationEvent
{
    public override string EventType => nameof(UserDeleted);
}
