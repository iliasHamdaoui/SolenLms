using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;

public sealed record UploadLectureVideoCommand(string ResourceId, IResourceFile File) : IRequest<RequestResponse>;