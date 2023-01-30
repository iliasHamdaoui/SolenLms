using Ardalis.Specification.EntityFrameworkCore;
using Imanys.SolenLms.Application.Learning.Features;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data;

internal sealed class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(LearningDbContext dbContext) : base(dbContext)
    {
    }
}
