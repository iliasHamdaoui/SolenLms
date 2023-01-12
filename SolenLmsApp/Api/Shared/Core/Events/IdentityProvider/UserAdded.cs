namespace Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;


public sealed record UserAdded(string OrganizationId, string UserId) : BaseIntegratedEvent
{
    public string GivenFamily { get; set; } = default!;
    public string FamilyFamily { get; set; } = default!;
    public string Email { get; set; } = default!;
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public override string EventType => nameof(UserAdded);
}