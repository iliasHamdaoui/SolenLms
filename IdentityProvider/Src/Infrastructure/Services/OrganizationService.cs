using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.IdentityProvider;
using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Imanys.SolenLms.IdentityProvider.Core.UseCases;
using Imanys.SolenLms.IdentityProvider.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Services;
internal sealed class OrganizationService : IOrganizationService
{
    private readonly IdentityDbContext _dbContext;
    private readonly IIntegrationEventsSender _eventsSender;

    public OrganizationService(IdentityDbContext dbContext, IIntegrationEventsSender eventsSender)
    {
        _dbContext = dbContext;
        _eventsSender = eventsSender;
    }

    public async Task DeleteOrganization(Organization organization, CancellationToken cancellationToken = default)
    {
        _dbContext.Organizations.Remove(organization);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _eventsSender.SendEvent(new OrganizationDeleted(organization.Id), cancellationToken);
    }

    public async Task<Organization?> GetTheCurrentUserOrganization(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Organizations
            .Include(x => x.Users)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetOrganizationUsers(string organizationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
             .Where(x => x.Organization.Id == organizationId)
             .AsNoTracking()
             .ToListAsync(cancellationToken);
    }

    public async Task UpdateOrganization(Organization organization, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<Organization>().Update(organization);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
