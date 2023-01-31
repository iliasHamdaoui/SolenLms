using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResources;
using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Data;

public sealed class ResourcesDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;

    public DbSet<LectureResource> Resources { get; set; } = default!;

    public ResourcesDbContext(DbContextOptions<ResourcesDbContext> options, ICurrentUser currentUser,
        IDateTime dateTime) : base(options)
    {
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<string>()
            .HaveMaxLength(200);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Resources");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ResourcesDbContext).Assembly);

        modelBuilder.Entity<LectureResource>().HasQueryFilter(x => x.OrganizationId == _currentUser.OrganizationId);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if (_currentUser.IsLoggedIn) // ensure we're not in a background service
                    {
                        entry.Entity.CreatedBy = _currentUser.UserId;
                        entry.Entity.LastModifiedBy = _currentUser.UserId;
                    }

                    entry.Entity.CreatedAt = _dateTime.Now;
                    entry.Entity.LastModifiedAt = entry.Entity.CreatedAt;
                    entry.Entity.Version = 1;
                    break;
                case EntityState.Modified:
                    if (_currentUser.IsLoggedIn) // ensure we're not in a background service
                    {
                        entry.Entity.LastModifiedBy = _currentUser.UserId;
                    }

                    entry.Entity.LastModifiedAt = _dateTime.Now;
                    entry.Entity.Version += 1;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);
        return result;
    }
}