using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand : IRequest<RequestResponse>
{
    public string CategoryName { get; set; } = default!;
}


