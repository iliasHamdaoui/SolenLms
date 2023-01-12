using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteLecture;

public sealed record DeleteLectureCommand(string CourseId, string ModuleId, string LectureId) : IRequest<RequestResponse>;
