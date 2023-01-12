using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery(string CourseId) : IRequest<RequestResponse<GetCourseByIdQueryResult>>;
