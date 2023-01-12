using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases;
public interface IOrganizationService
{
    Task<Organization?> GetTheCurrentUserOrganization(CancellationToken cancellationToken = default);
    Task UpdateOrganization(Organization organization, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetOrganizationUsers(string organizationId, CancellationToken cancellationToken = default);
    Task DeleteOrganization(Organization organization, CancellationToken cancellationToken = default);
}
