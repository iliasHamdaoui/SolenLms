using Fluxor;
using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.Courses.Store;

public sealed class Effects
{
    private readonly IState<LearningCoursesState> _state;
    private readonly IWebApiClient _webApiClient;
    private readonly NotificationsService _notificationsService;
    private readonly ILogger<Effects> _logger;

    public Effects(IState<LearningCoursesState> state, IWebApiClient webApiClient, NotificationsService notificationsService,
             ILogger<Effects> logger)
    {
        _state = state;
        _webApiClient = webApiClient;
        _notificationsService = notificationsService;
        _logger = logger;
    }

    [EffectMethod]
    public async Task OnLoadCoursesAction(LoadCoursesAction action, IDispatcher dispatcher)
    {
        var (page, pageSize, orderBy, categoriesIds, referentsIds, bookmarkOnly) = _state.Value.GetCoursesQuery;
        try
        {
            var coursesResultTask = _webApiClient.GetAllCoursesAsync(page, pageSize, orderBy, categoriesIds, referentsIds, bookmarkOnly, action.CancellationToken);

            var filtersResultTask = _webApiClient.GetFiltersAsync(action.CancellationToken);

            await Task.WhenAll(coursesResultTask, filtersResultTask);


            var coursesResult = await coursesResultTask;
            var filtersResult = await filtersResultTask;

            dispatcher.Dispatch(new LoadCoursesFiltersResultAction(filtersResult.Data.Categories, filtersResult.Data.Instructors));
            dispatcher.Dispatch(new LoadCoursesResultAction(coursesResult.Data.Courses, coursesResult.Data.CourseTotalCount));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            _notificationsService.ShowErreur(exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
        }
    }
}
