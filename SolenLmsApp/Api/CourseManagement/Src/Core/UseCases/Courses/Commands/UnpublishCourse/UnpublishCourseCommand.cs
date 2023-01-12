using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.UnpublishCourse;

public sealed record UnpublishCourseCommand(string CourseId) : IRequest<RequestResponse>;
