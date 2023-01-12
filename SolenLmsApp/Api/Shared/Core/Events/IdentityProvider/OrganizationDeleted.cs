namespace Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
public sealed record OrganizationDeleted(string OrganizationId) : BaseIntegratedEvent
{
    public override string EventType => nameof(OrganizationDeleted);
}
