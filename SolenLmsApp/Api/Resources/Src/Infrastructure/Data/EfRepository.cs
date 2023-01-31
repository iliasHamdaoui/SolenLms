using Ardalis.Specification.EntityFrameworkCore;
using Imanys.SolenLms.Application.Resources.Features;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Data;

internal sealed class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(ResourcesDbContext dbContext) : base(dbContext)
    {
    }
}
