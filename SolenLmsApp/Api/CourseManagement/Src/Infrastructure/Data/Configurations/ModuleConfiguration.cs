using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data.Configurations;

internal sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.Property(x => x.Title)
            .IsRequired();

        builder.HasMany(x => x.Lectures)
            .WithOne()
            .HasForeignKey(x => x.ModuleId)
            .IsRequired();
    }
}
