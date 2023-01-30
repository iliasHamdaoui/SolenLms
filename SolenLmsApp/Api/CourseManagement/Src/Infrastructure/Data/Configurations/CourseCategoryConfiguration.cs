using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseCategoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data.Configurations;
internal sealed class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory>
{
    public void Configure(EntityTypeBuilder<CourseCategory> builder)
    {
        builder.HasKey(x => new { x.CourseId, x.CategoryId });

        builder.HasOne(x => x.Course)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.CourseId);

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId);
    }
}
