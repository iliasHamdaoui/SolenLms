using Imanys.SolenLms.Application.Learning.Core.Domain.Learners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;
internal sealed class LearnerConfiguration : IEntityTypeConfiguration<Learner>
{
    public void Configure(EntityTypeBuilder<Learner> builder)
    {
        builder.HasIndex(x => x.OrganizationId);
    }
}
