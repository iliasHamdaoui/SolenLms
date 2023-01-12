using Fluxor;
using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.CourseDetail.Store;

public sealed class Effects
{
    private readonly ICoursesClient _coursesClient;
    private readonly ICategoriesClient _categoriesClient;
    private readonly ICoursesCategoriesClient _coursesCategoriesClient;
    private readonly NotificationsService _notificationsService;
    private readonly NavigationManager _navigationManager;
    private readonly ILearnersProgressClient _coursesLearnersClient;
    private readonly ILogger<Effects> _logger;

    public Effects(ICoursesClient coursesClient, ICategoriesClient categoriesClient, ICoursesCategoriesClient coursesCategoriesClient, NotificationsService notificationsService,
    NavigationManager navigationManager, ILearnersProgressClient coursesLearnersClient, ILogger<Effects> logger)
    {
        _coursesClient = coursesClient;
        _categoriesClient = categoriesClient;
        _coursesCategoriesClient = coursesCategoriesClient;
        _notificationsService = notificationsService;
        _navigationManager = navigationManager;
        _coursesLearnersClient = coursesLearnersClient;
        _logger = logger;
    }



    [EffectMethod]
    public async Task OnLoadCourseAction(LoadCourseAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _coursesClient.GetCourseByIdAsync(action.CourseId, action.CancellationToken);
            dispatcher.Dispatch(new LoadCourseResultAction(result.Data));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            if (exception.StatusCode == 404)
            {
                _navigationManager.NavigateTo("not-found");
                return;
            }

            _notificationsService.ShowErreur(exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
        }
    }

    [EffectMethod]
    public async Task OnLoadCategoriesAction(LoadCourseCategoriesAction action, IDispatcher dispatcher)
    {
        try
        {
            var categoriesResult = await _categoriesClient.GetAllCategoriesAsync(action.CancellationToken);
            var courseCategoriesResult = await _coursesCategoriesClient.GetCourseCategoriesIdsAsync(action.CourseId, action.CancellationToken);
            dispatcher.Dispatch(new LoadCourseCategoriesResultAction(categoriesResult.Data.Categories, courseCategoriesResult.Data));
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
    public async Task OnLoadLearnersAction(LoadLearnersAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _coursesLearnersClient.GetLearnersProgressAsync(action.CourseId, action.CancellationToken);
            dispatcher.Dispatch(new LoadLearnersResultAction(result.Data.Learners));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            if (exception.StatusCode == 404)
            {
                _navigationManager.NavigateTo("not-found");
                return;
            }

            _notificationsService.ShowErreur(exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
        }
    }
}
