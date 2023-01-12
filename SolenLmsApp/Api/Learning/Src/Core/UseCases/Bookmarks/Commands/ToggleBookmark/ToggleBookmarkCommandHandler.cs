using Imanys.SolenLms.Application.Learning.Core.Domain.BookmarkAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Microsoft.Extensions.Logging;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Bookmarks.Commands.ToggleBookmark;

internal sealed class ToggleBookmarkCommandHandler : IRequestHandler<ToggleBookmarkCommand, RequestResponse>
{
    private readonly IRepository<BookmarkedCourse> _bookmarkRepo;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ToggleBookmarkCommandHandler> _logger;

    public ToggleBookmarkCommandHandler(IRepository<BookmarkedCourse> bookmarkRepo, ICurrentUser currentUser,
        ILogger<ToggleBookmarkCommandHandler> logger)
    {
        _bookmarkRepo = bookmarkRepo;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(ToggleBookmarkCommand command, CancellationToken _)
    {
        try
        {
            BookmarkedCourse? learnerBookmarkedCourse = await GetBookmarkedCourseFromRepository(command.CourseId);

            if (CourseIsAlreadyBookmarked(learnerBookmarkedCourse))
            {
                await DeleteBookmarkFromRepository(learnerBookmarkedCourse!);
                return Ok("Unbookmarked");
            }

            await SaveBookmarkToRepository(command.CourseId);
            return Ok("Bookmarked");
        }
        catch (Exception ex)
        {
            return UnexpectedError("while toggling course bookmarking.", ex);
        }
    }

    #region private methods

    private async Task DeleteBookmarkFromRepository(BookmarkedCourse learnerBookmarkedCourse)
    {
        await _bookmarkRepo.DeleteAsync(learnerBookmarkedCourse);
    }

    private async Task SaveBookmarkToRepository(string courseId)
    {
        await _bookmarkRepo.AddAsync(new BookmarkedCourse(_currentUser.UserId, courseId));
    }

    private async Task<BookmarkedCourse?> GetBookmarkedCourseFromRepository(string courseId)
    {
        return await _bookmarkRepo.FirstOrDefaultAsync(new GetLearnerBookmarkedCourse(_currentUser.UserId, courseId));
    }

    private static bool CourseIsAlreadyBookmarked(BookmarkedCourse? learnerBookmarkedCourse)
    {
        return learnerBookmarkedCourse is not null;
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "A error occured while toggling course bookmarking. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }


    private sealed class GetLearnerBookmarkedCourse : Specification<BookmarkedCourse>
    {
        public GetLearnerBookmarkedCourse(string learnerId, string courseId)
        {
            Query
                .Where(x => x.LearnerId == learnerId && x.CourseId == courseId);
        }
    }

    #endregion
}