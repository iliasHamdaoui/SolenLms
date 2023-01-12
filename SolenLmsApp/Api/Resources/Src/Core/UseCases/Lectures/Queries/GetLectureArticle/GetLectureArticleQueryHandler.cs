using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.GetLectureArticle;


internal sealed class GetLectureArticleQueryHandler : IRequestHandler<GetLectureArticleQuery, RequestResponse<string>>
{
    private readonly IRepository<LectureResource> _repository;
    private readonly IHashids _hashids;

    public GetLectureArticleQueryHandler(IRepository<LectureResource> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }
    public async Task<RequestResponse<string>> Handle(GetLectureArticleQuery query, CancellationToken cancellationToken)
    {

        if (!_hashids.TryDecodeSingle(query.ResourceId, out int resourceId))
            return RequestResponse<string>.Error(ResponseError.NotFound, "Invalid resource id.");

        var resource = await _repository.GetByIdAsync(resourceId, cancellationToken);
        if (resource == null)
            return RequestResponse<string>.Error(ResponseError.NotFound, "The resource does not exist.");

        if (resource.MediaType.Value != MediaType.Text.Value)
            return RequestResponse<string>.Error(ResponseError.NotFound, "The article format is incorrect.");

        return RequestResponse<string>.Ok(data: resource.Data);
    }
}
