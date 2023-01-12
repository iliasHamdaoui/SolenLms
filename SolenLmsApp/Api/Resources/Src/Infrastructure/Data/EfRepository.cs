using Ardalis.Specification.EntityFrameworkCore;
using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Data;

internal sealed class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(ResourcesDbContext dbContext) : base(dbContext)
    {
    }
}
