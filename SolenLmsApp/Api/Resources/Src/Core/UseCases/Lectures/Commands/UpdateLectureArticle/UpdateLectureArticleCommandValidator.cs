namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UpdateLectureArticle;


public sealed class UpdateLectureArticleCommandValidator : AbstractValidator<UpdateLectureArticleCommand>
{
    public UpdateLectureArticleCommandValidator()
    {
        RuleFor(x => x.ResourceId).NotEmpty();
        RuleFor(x => x.Content).MaximumLength(10000);
    }
}
