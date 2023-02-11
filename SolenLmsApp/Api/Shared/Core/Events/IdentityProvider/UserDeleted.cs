namespace Imanys.SolenLms.Application.Shared.Core.Events;

public sealed record UserDeleted(string UserId) : BaseIntegrationEvent
{
    public override string EventType => nameof(UserDeleted);
}
