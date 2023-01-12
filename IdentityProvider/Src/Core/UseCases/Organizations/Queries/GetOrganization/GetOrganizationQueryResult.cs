namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganization;
public sealed record GetOrganizationQueryResult
{
    public string OrganizationName { get; set; } = default!;
    public DateTime CreationDate { get; set; } = default!;
    public int UserCount { get; set; }
}
