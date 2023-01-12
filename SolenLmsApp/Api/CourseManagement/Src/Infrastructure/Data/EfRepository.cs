using Ardalis.Specification.EntityFrameworkCore;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;

internal sealed class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
{
    public EfRepository(CourseManagementDbContext dbContext) : base(dbContext)
    {
    }
}