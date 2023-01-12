using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Data;

public sealed class IdentityDbContext : IdentityDbContext<User>
{
    private readonly ICurrentUser _currentUser;

    public DbSet<Organization> Organizations { get; set; } = default!;
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, ICurrentUser currentUser)
         : base(options)
    {
        _currentUser = currentUser;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder
        //    .Properties<string>()
        //    .HaveMaxLength(200);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        modelBuilder.Entity<Organization>().HasQueryFilter(x => x.Id == _currentUser.OrganizationId);
    }
}
