using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.CheckLectureContent;


internal sealed class CheckLectureContentQueryHandler : IRequestHandler<CheckLectureContentQuery, RequestResponse<bool>>
{
    private readonly IRepository<LectureResource> _repository;
    private readonly IHashids _hashids;

    public CheckLectureContentQueryHandler(IRepository<LectureResource> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }
    public async Task<RequestResponse<bool>> Handle(CheckLectureContentQuery query, CancellationToken cancellationToken)
    {

        if (!_hashids.TryDecodeSingle(query.ResourceId, out int resourceId))
            return RequestResponse<bool>.Error(ResponseError.NotFound, "Invalid resource id.");

        var resource = await _repository.GetByIdAsync(resourceId, cancellationToken);
        if (resource == null)
            return RequestResponse<bool>.Error(ResponseError.NotFound, "The resource does not exist.");

        return RequestResponse<bool>.Ok(data: resource.Data != null);
    }
}