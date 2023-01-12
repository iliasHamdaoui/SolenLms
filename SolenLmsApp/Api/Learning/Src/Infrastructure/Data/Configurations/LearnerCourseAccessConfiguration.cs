using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;
internal sealed class LearnerCourseAccessConfiguration : IEntityTypeConfiguration<LearnerCourseAccess>
{
    public void Configure(EntityTypeBuilder<LearnerCourseAccess> builder)
    {
        builder.HasKey(x => new { x.LearnerId, x.LectureId });
    }
}
