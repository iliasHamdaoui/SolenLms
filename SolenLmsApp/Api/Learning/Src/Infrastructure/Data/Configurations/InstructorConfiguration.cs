using Imanys.SolenLms.Application.Learning.Core.Domain.InstructorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;

internal sealed class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {

        builder.Property(x => x.GivenName)
            .IsRequired();

        builder.Property(x => x.FamilyName)
            .IsRequired();

        builder.HasIndex(x => x.OrganizationId);
    }
}
