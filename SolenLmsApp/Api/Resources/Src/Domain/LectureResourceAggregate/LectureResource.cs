using Imanys.SolenLms.Application.Shared.Core.Entities;
using Imanys.SolenLms.Application.Shared.Core.Enums;

namespace Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;

public class LectureResource : BaseAuditEntity, IAggregateRoot
{
    public LectureResource(string organizationId, string courseId, string moduleId, string lectureId,
        MediaType mediaType)
    {
        ArgumentNullException.ThrowIfNull(nameof(organizationId));
        ArgumentNullException.ThrowIfNull(nameof(courseId));
        ArgumentNullException.ThrowIfNull(nameof(moduleId));
        ArgumentNullException.ThrowIfNull(nameof(lectureId));
        ArgumentNullException.ThrowIfNull(nameof(mediaType));

        OrganizationId = organizationId;
        MediaType = mediaType;
        CourseId = courseId;
        ModuleId = moduleId;
        LectureId = lectureId;
    }

    public string OrganizationId { get; init; }
    public string CourseId { get; init; }
    public string ModuleId { get; init; }
    public string LectureId { get; init; }
    public MediaType MediaType { get; init; }
    public string? Data { get; private set; }
    public long Size { get; private set; }

    public void UpdateData(string? data, long size = 0)
    {
        Size = size;
        Data = data;
    }
}