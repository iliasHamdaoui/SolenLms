using Imanys.SolenLms.Application.Learning.Core.Domain.LearnersProgress;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;
internal class LearnerCourseProgressConfiguration : IEntityTypeConfiguration<LearnerCourseProgress>
{
    public void Configure(EntityTypeBuilder<LearnerCourseProgress> builder)
    {

        builder.HasKey(x => new { x.LearnerId, x.CourseId });
    }
}
