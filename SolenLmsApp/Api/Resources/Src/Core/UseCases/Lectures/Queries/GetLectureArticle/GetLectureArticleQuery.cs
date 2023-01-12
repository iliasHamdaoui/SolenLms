using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.GetLectureArticle;


public sealed record GetLectureArticleQuery : IRequest<RequestResponse<string>>
{
    public string ResourceId { get; }

    public GetLectureArticleQuery(string resourceId)
    {
        ResourceId = resourceId;
    }
}