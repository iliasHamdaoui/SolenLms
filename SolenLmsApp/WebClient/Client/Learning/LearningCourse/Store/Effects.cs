using Fluxor;
using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared;
using Imanys.SolenLms.Application.WebClient.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.Application.WebClient.Learning.LearningCourse.Store;

public sealed class Effects
{
    private readonly NotificationsService _notificationsService;
    private readonly ILearningClient _learningClient;
    private readonly IState<LearningState> _state;
    private readonly IResourcesClient _resourcesClient;
    private readonly ILogger<Effects> _logger;
    private readonly VideoResourcesUrl _videoResourcesUrl;
    private readonly NavigationManager _navigationManager;

    public Effects(NotificationsService notificationsService, ILearningClient learningClient, IState<LearningState> state,
    IResourcesClient resourcesClient, ILogger<Effects> logger, IOptions<VideoResourcesUrl> videoResourcesUrlOptions, NavigationManager navigationManager)
    {
        _notificationsService = notificationsService;
        _learningClient = learningClient;
        _state = state;
        _resourcesClient = resourcesClient;
        _logger = logger;
        _videoResourcesUrl = videoResourcesUrlOptions.Value;
        _navigationManager = navigationManager;
    }

    [EffectMethod]
    public async Task OnLoadCourseAction(LoadLearningCourseAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _learningClient.GetCourseAsync(action.CourseId, action.CancellationToken);
            dispatcher.Dispatch(new LoadLearningCourseResultAction(result.Data));
            dispatcher.Dispatch(new LoadLectureAction(action.CourseId, result.Data.FirstLecture, action.CancellationToken));
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
    public async Task OnLoadLectureAction(LoadLectureAction action, IDispatcher dispatcher)
    {
        try
        {
            var lecture = _state.Value?.Course?.Modules.SelectMany(x => x.Lectures).FirstOrDefault(x => x.Id == action.LectureId);
            if (lecture == null)
                return;
            if (lecture.ResourceId != null)
            {
                if (lecture.LectureType == "Article")
                {
                    var resourceContent = await _resourcesClient.GetArticleAsync(lecture.ResourceId, action.CancellationToken);
                    lecture.Content = resourceContent.Data;
                }
                else if (lecture.LectureType == "Video")
                {
                    lecture.Content = $"{_videoResourcesUrl.Value}/{lecture.ResourceId}";
                }
            }

           await _learningClient.UpdateLearnerProgressAsync(action.CourseId, action.LectureId, IsTheLastLectureOfTheCourse(lecture), action.CancellationToken);

            dispatcher.Dispatch(new LoadLectureResultAction(lecture));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            _notificationsService.ShowErreur(exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "{message}", exception.Message);
        }

        static bool IsTheLastLectureOfTheCourse(LectureForGetCourseToLearnByIdQueryResult lecture)
        {
            return lecture.NextLectureId == null;
        }
    }
}

