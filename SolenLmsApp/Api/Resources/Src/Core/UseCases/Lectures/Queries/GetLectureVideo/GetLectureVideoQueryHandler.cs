using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Queries.GetLectureVideo;

internal sealed class GetLectureVideoQueryHandler : IRequestHandler<GetLectureVideoQuery, RequestResponse<Stream>>
{
    private readonly IRepository<LectureResource> _repository;
    private readonly IMediaManager _videoManager;
    private readonly IHashids _hashids;

    public GetLectureVideoQueryHandler(IRepository<LectureResource> repository, IMediaManager videoManager, IHashids hashids)
    {
        _repository = repository;
        _videoManager = videoManager;
        _hashids = hashids;
    }
    public async Task<RequestResponse<Stream>> Handle(GetLectureVideoQuery query, CancellationToken cancellationToken)
    {

        if (!_hashids.TryDecodeSingle(query.ResourceId, out int resourceId))
            return RequestResponse<Stream>.Error(ResponseError.NotFound, "Invalid resource id.");

        var resource = await _repository.GetByIdAsync(resourceId, cancellationToken);
        if (resource == null)
            return RequestResponse<Stream>.Error(ResponseError.NotFound, "The resource does not exist.");

        if (resource.MediaType.Value != MediaType.Video.Value)
            return RequestResponse<Stream>.Error(ResponseError.NotFound, "The video format is incorrect.");

        var stream = await _videoManager.GetMediaContentStream(resource.Data);

        if(stream == null)
            return RequestResponse<Stream>.Error(ResponseError.NotFound, "The resource does not exist.");

        return RequestResponse<Stream>.Ok(data: stream);
    }
}
