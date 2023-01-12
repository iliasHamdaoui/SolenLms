using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;

internal sealed class ModuleConfiguration : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {

        builder.Property(x => x.Title)
            .IsRequired();

        builder.HasMany(x => x.Lectures)
            .WithOne(x => x.Module)
            .IsRequired();
    }
}
