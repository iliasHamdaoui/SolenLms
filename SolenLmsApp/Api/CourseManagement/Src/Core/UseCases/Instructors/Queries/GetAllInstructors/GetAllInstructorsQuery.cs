using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Instructors.Queries.GetAllInstructors;

public sealed record GetAllInstructorsQuery : IRequest<RequestResponse<GetAllInstructorsQueryResult>>
{
}

