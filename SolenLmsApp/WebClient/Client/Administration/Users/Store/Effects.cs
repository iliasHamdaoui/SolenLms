using Fluxor;
using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Administration.Users.Store;

public sealed class Effects
{
    private readonly IUsersClient _usersClient;
    private readonly NotificationsService _notificationsService;
    private readonly ILogger<Effects> _logger;

    public Effects(IUsersClient usersClient, NotificationsService notificationsService, ILogger<Effects> logger)
    {
 
        _usersClient = usersClient;
        _notificationsService = notificationsService;
        _logger = logger;
    }
    [EffectMethod]
    public async Task OnLoadUsersAction(LoadUsersAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _usersClient.GetUsersAsync(action.CancellationToken);

            dispatcher.Dispatch(new LoadUsersResultAction(result.Data));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            _notificationsService.ShowErreur(exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "Error occurend while fetching users, {message}", exception.Message);
        }
    }
}
