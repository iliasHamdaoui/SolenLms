namespace Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
public sealed record OrganizationDeleted(string OrganizationId) : BaseIntegrationEvent
{
    public override string EventType => nameof(OrganizationDeleted);
}
