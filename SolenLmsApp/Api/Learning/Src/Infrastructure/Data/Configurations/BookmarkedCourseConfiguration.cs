using Imanys.SolenLms.Application.Learning.Core.Domain.BookmarkAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.Data.Configurations;

internal sealed class BookmarkedCourseConfiguration : IEntityTypeConfiguration<BookmarkedCourse>
{
    public void Configure(EntityTypeBuilder<BookmarkedCourse> builder)
    {
        builder.HasKey(x => new { x.LearnerId, x.CourseId });
    }
}
