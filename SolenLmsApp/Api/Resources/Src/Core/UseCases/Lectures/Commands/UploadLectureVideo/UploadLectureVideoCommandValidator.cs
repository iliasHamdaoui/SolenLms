namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;

public sealed class UploadLectureVideoCommandValidator : AbstractValidator<UploadLectureVideoCommand>
{
    public UploadLectureVideoCommandValidator()
    {
        RuleFor(x => x.ResourceId).NotEmpty();
        RuleFor(x => x.File).NotNull();
        RuleFor(x => x.File.ContentType).NotEmpty();
    }
}
