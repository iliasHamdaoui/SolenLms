using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Data.Repositories.Storage;
internal sealed class StorageRepo : IStorageRepo
{
    private readonly ResourcesDbContext _dbContext;

    public StorageRepo(ResourcesDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<long> GetCurrentStorageRepo(string organizationId, CancellationToken cancellationToken)
    {
        return _dbContext.Resources.Where(x => x.OrganizationId == organizationId).SumAsync(x => x.Size, cancellationToken);
    }
}
