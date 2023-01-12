using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Bookmarks.Commands.ToggleBookmark;

public sealed record ToggleBookmarkCommand(string CourseId) : IRequest<RequestResponse>;
