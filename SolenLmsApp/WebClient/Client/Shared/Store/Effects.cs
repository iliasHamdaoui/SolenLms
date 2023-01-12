using Fluxor;
using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Shared.Store;

public sealed class Effects
{

    private readonly IOrganizationsClient _organizationsClient;
    private readonly ILogger<Effects> _logger;

    public Effects(IOrganizationsClient organizationsClient, ILogger<Effects> logger)
    {

        _organizationsClient = organizationsClient;
        _logger = logger;
    }
    [EffectMethod]
    public async Task OnLoadOrganizationNameAction(LoadOrganizationNameAction action, IDispatcher dispatcher)
    {
        try
        {
            var result = await _organizationsClient.GetOrganizationNameAsync();

            dispatcher.Dispatch(new LoadOrganizationNameResultAction(result.Data));
        }
        catch (ApiException<ProblemDetails> exception)
        {
            _logger.LogError(exception, "Error occurend while fetching organization name, {message}", exception.Result.Detail);
        }
        catch (ApiException exception)
        {
            _logger.LogError(exception, "Error occurend while fetching organization name, {message}", exception.Message);
        }
    }
}
