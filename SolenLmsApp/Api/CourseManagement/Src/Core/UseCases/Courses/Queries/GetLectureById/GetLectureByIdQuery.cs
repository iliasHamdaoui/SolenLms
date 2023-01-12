using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetLectureById;

public sealed record GetLectureByIdQuery
    (string CourseId, string ModuleId, string LectureId) : IRequest<RequestResponse<GetLectureByIdQueryResult>>;