using Fluxor;
using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;
using Imanys.SolenLms.Application.WebClient.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Administration.Organization.Store;

public sealed class Effects
{
    private readonly IOrganizationsClient _organizationsClient;
    private readonly NotificationsService _notificationsService;
    private readonly ILogger<Effects> _logger;

    public Effects(IOrganizationsClient organizationsClient, NotificationsService notificationsService, ILogger<Effects> logger)
    {
        _organizationsClient = organizationsClient;
        _notificationsService = notificationsService;
        _logger = logger;
    }
    [EffectMethod]
    public async Task OnLoadOrganizationAction(LoadOrganizationAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _organizationsClient.GetOrganizationAsync(action.CancellationToken);

            dispatcher.Dispatch(new LoadOrganizationResultAction(result.Data));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            _notificationsService.ShowErreur(exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "Error occurend while fetching organization info, {message}", exception.Message);
        }
    }
}
