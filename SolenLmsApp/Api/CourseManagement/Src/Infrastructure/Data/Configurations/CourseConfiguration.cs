using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data.Configurations;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(x => x.Title)
        .IsRequired();

        builder.HasMany(x => x.Modules)
        .WithOne()
        .HasForeignKey(x => x.CourseId)
        .IsRequired();

        builder.HasIndex(x => x.OrganizationId);

        builder.HasOne(x => x.Instructor)
            .WithMany()
            .HasForeignKey(x => x.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.Version).IsConcurrencyToken();
    }
}
