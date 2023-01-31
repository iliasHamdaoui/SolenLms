using Imanys.SolenLms.Application.Learning.Core.Domain.Courses;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;

internal sealed class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {

        builder.Property(x => x.Title)
        .IsRequired();


        builder.Property(x => x.Type)
           .HasMaxLength(200)
           .HasConversion(x => x.Value, x => Enumeration.FromValue<LectureType>(x).item);
    }
}
