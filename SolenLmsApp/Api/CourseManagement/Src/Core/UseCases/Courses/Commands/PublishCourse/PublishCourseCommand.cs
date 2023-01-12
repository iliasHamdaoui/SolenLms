using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.PublishCourse;

public sealed record PublishCourseCommand(string CourseId) : IRequest<RequestResponse>;
