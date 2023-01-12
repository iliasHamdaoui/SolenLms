using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetModuleById;

public sealed record GetModuleByIdQuery : IRequest<RequestResponse<GetModuleByIdQueryResult>>
{
    public GetModuleByIdQuery(string courseId, string moduleId)
    {
        CourseId = courseId;
        ModuleId = moduleId;
    }

    public string CourseId { get; }
    public string ModuleId { get; }
}
