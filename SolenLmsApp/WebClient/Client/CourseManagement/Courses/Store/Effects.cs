using Fluxor;
using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.Courses.Store;

public sealed class Effects
{
    private readonly ICoursesClient _coursesClient;
    private readonly ICategoriesClient _categoriesClient;
    private readonly IInstructorsClient _instructorsClient;
    private readonly IState<CoursesState> _state;
    private readonly NotificationsService _notificationsService;
    private readonly ILogger<Effects> _logger;

    public Effects(ICoursesClient coursesClient, ICategoriesClient categoriesClient,
    IInstructorsClient instructorsClient, IState<CoursesState> state, NotificationsService notificationsService, ILogger<Effects> logger)
    {
        _coursesClient = coursesClient;
        _categoriesClient = categoriesClient;
        _instructorsClient = instructorsClient;
        _state = state;
        _notificationsService = notificationsService;
        _logger = logger;
    }

    [EffectMethod]
    public async Task OnLoadCoursesAction(LoadCoursesAction action, IDispatcher dispatcher)
    {
        var (page, pageSize, orderBy, isSortDescending, categoriesIds, referentsIds) = _state.Value.GetCoursesQuery;
        try
        {
            var result = await _coursesClient.GetAllCoursesAsync(page, pageSize, orderBy, isSortDescending, categoriesIds, referentsIds, action.CancellationToken);
            dispatcher.Dispatch(new LoadCoursesResultAction(result.Data.Courses, result.Data.CourseTotalCount));
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

    [EffectMethod]
    public async Task OnLoadFiltersAction(LoadFiltersAction action, IDispatcher dispatcher)
    {
        try
        {
            var categoriesResult = await _categoriesClient.GetAllCategoriesAsync(action.CancellationToken);
            var referentsResult = await _instructorsClient.GetAllInstructorsAsync(action.CancellationToken);
            dispatcher.Dispatch(new LoadFiltersResultAction(categoriesResult.Data.Categories, referentsResult.Data.Referents));
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
