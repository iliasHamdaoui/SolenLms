namespace Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;

public sealed record UserDeleted(string UserId) : BaseIntegratedEvent
{
    public override string EventType => nameof(UserDeleted);
}
