using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Categories;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CoursesCategories;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Instructors;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Learners;
using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnersProgress;
using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;

public class CourseManagementDbContext : DbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _dateTime;

    public DbSet<Course> Courses { get; set; } = default!;
    public DbSet<Lecture> Lecture { get; set; } = default!;
    public DbSet<Instructor> Instructors { get; set; } = default!;
    public DbSet<Category> Categories { get; set; } = default!;
    public DbSet<Learner> Learners { get; set; } = default!;
    public DbSet<CourseCategory> CourseCategories { get; set; } = default!;
    public DbSet<LearnerCourseProgress> LearnerProgress { get; set; } = default!;

    public CourseManagementDbContext(DbContextOptions<CourseManagementDbContext> options, ICurrentUser currentUser,
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
        modelBuilder.HasDefaultSchema("CourseManagement");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseManagementDbContext).Assembly);

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
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}