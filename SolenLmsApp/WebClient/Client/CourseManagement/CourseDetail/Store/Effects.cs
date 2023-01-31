using Fluxor;
using Imanys.SolenLms.Application.WebClient.CourseManagement.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.CourseDetail.Store;

public sealed class Effects
{
    private readonly IApiClient _apiClient;
    private readonly NotificationsService _notificationsService;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<Effects> _logger;

    public Effects(IApiClient apiClient, NotificationsService notificationsService, NavigationManager navigationManager,
        ILogger<Effects> logger)
    {
        _apiClient = apiClient;
        _notificationsService = notificationsService;
        _navigationManager = navigationManager;
        _logger = logger;
    }


    [EffectMethod]
    public async Task OnLoadCourseAction(LoadCourseAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _apiClient.GetCourseByIdAsync(action.CourseId, action.CancellationToken);
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
            var categoriesResult = await _apiClient.GetAllCategoriesAsync(action.CancellationToken);
            var courseCategoriesResult =
                await _apiClient.GetCourseCategoriesIdsAsync(action.CourseId, action.CancellationToken);
            dispatcher.Dispatch(new LoadCourseCategoriesResultAction(categoriesResult.Data.Categories,
                courseCategoriesResult.Data));
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
            var result = await _apiClient.GetLearnersProgressAsync(action.CourseId, action.CancellationToken);
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