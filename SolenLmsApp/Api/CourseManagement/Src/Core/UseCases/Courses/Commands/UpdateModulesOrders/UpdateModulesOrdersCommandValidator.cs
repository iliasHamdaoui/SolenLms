namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UpdateModulesOrders;

public sealed class UpdateModulesOrdersCommandValidator : AbstractValidator<UpdateModulesOrdersCommand>
{
	public UpdateModulesOrdersCommandValidator()
	{
		RuleFor(x => x.CourseId).NotEmpty();
		RuleFor(x => x.ModulesOrders).NotNull();
	}
}
