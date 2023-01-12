using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.CheckLectureContent;

public sealed record CheckLectureContentQuery : IRequest<RequestResponse<bool>>
{
    public string ResourceId { get; }

    public CheckLectureContentQuery(string resourceId)
    {
        ResourceId = resourceId;
    }
}
