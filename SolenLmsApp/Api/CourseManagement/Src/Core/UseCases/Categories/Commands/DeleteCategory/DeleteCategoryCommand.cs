using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(int CategoryId) : IRequest<RequestResponse>;
