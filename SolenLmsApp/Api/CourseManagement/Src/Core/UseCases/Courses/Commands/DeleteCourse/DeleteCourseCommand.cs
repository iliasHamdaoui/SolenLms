using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteCourse;

public sealed record DeleteCourseCommand(string CourseId) : IRequest<RequestResponse>;
