using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.GetLectureVideo;

public sealed record GetLectureVideoQuery : IRequest<RequestResponse<Stream>>
{
    public string ResourceId { get; }


    public GetLectureVideoQuery(string resourceId)
    {
        ResourceId = resourceId;
    }

}
