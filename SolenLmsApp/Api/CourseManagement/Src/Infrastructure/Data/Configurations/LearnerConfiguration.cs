using Imanys.SolenLms.Application.CourseManagement.Core.Domain.LearnerAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data.Configurations;
internal sealed class LearnerConfiguration : IEntityTypeConfiguration<Learner>
{
    public void Configure(EntityTypeBuilder<Learner> builder)
    {
        builder.HasIndex(x => x.OrganizationId);
    }
}
