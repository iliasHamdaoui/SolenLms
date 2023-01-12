namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Commands.UpdateCourseCategories;

public sealed class UpdateCourseCategoriesCommandValidator : AbstractValidator<UpdateCourseCategoriesCommand>
{
    public UpdateCourseCategoriesCommandValidator()
    {
        RuleFor(x => x.SelectecdCategroriesIds).NotNull();
    }
}
