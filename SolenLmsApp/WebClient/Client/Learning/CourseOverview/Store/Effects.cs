using Fluxor;
using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace Imanys.SolenLms.Application.WebClient.Learning.CourseOverview.Store;

public sealed class Effects
{

    private readonly IWebApiClient _webApiClient;
    private readonly NotificationsService _notificationsService;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<Effects> _logger;

    public Effects(IWebApiClient webApiClient, NotificationsService notificationsService,
                NavigationManager navigationManager, ILogger<Effects> logger)
    {
        _webApiClient = webApiClient;
        _notificationsService = notificationsService;
        _navigationManager = navigationManager;
        _logger = logger;
    }


    [EffectMethod]
    public async Task OnLoadCourseAction(LoadCourseAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _webApiClient.GetCourseByIdAsync(action.CourseId, action.CancellationToken);
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
}
