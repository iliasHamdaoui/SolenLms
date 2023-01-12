using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.DeleteModule;

public sealed record DeleteModuleCommand(string CourseId, string ModuleId) : IRequest<RequestResponse>;
