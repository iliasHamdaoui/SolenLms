using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResources;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Data.Configurations;

internal sealed class LectureResourceConfiguration : IEntityTypeConfiguration<LectureResource>
{
    public void Configure(EntityTypeBuilder<LectureResource> builder)
    {

        builder.ToTable("LectureResources");

        builder.Property(x => x.OrganizationId)
            .IsRequired();

        builder.Property(x => x.LectureId)
            .IsRequired();

        builder.Property(x => x.MediaType)
            .IsRequired()
           .HasConversion(x => x.Value, x => Enumeration.FromValue<MediaType>(x).item);

        builder.Property(x => x.Data)
           .HasMaxLength(10000);


        builder.Property(x => x.Version).IsConcurrencyToken();
    }
}
