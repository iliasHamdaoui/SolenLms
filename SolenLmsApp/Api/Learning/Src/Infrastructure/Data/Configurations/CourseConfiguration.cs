using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(x => x.Title)
        .IsRequired();

        builder.HasMany(x => x.Modules)
        .WithOne(x => x.Course)
        .IsRequired();

        builder.HasOne(x => x.Instructor)
        .WithMany()
        .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.OrganizationId);

        builder.HasIndex(x => x.IsPublished);
    }
}
