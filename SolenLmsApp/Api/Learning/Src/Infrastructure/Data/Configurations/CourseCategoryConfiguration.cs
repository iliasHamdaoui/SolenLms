using Imanys.SolenLms.Application.Learning.Core.Domain.CoursesCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;
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
