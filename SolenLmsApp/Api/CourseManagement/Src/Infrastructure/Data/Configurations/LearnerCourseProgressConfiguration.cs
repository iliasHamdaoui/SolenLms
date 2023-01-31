using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnersProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data.Configurations;
internal class LearnerCourseProgressConfiguration : IEntityTypeConfiguration<LearnerCourseProgress>
{
    public void Configure(EntityTypeBuilder<LearnerCourseProgress> builder)
    {
        builder.HasKey(x => new { x.LearnerId, x.CourseId });
    }
}
