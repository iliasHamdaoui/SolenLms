using Imanys.SolenLms.Application.Learning.Core.Domain.CategoryAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.CourseCategory;
using Imanys.SolenLms.Application.Learning.Core.Domain.InstructorAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;
using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data;

public sealed class LearningDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;
    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Learner> Learners { get; set; } = default!;
    public DbSet<Instructor> Instructors { get; set; } = default!;
    public DbSet<CourseCategory> CourseCategories { get; set; } = default!;
    public DbSet<LearnerCourseAccess> LearnerCourseAccess { get; set; } = default!;
    public DbSet<LearnerCourseProgress> LearnerCourseProgress { get; set; } = default!;

    public LearningDbContext(DbContextOptions<LearningDbContext> options, ICurrentUser currentUser, IDateTime dateTime)
        : base(options)
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
        modelBuilder.HasDefaultSchema("Learning");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearningDbContext).Assembly);

        modelBuilder.Entity<Course>().HasQueryFilter(x => x.OrganizationId == _currentUser.OrganizationId);
        modelBuilder.Entity<Instructor>().HasQueryFilter(x => x.OrganizationId == _currentUser.OrganizationId);
        modelBuilder.Entity<Learner>().HasQueryFilter(x => x.OrganizationId == _currentUser.OrganizationId);
        modelBuilder.Entity<Category>().HasQueryFilter(x => x.OrganizationId == _currentUser.OrganizationId);
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