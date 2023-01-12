using Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.IdentityProvider.Infrastructure.Data.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FamilyName).HasMaxLength(200);
        builder.Property(x => x.GivenName).HasMaxLength(200);
    }
}
