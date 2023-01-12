using Microsoft.AspNetCore.Components;

namespace Imanys.SolenLms.Application.WebClient.Shared.Bff;

internal sealed class UnauthorizedMessageHandler : DelegatingHandler
{

    private readonly NavigationManager _navigationManager;



    public UnauthorizedMessageHandler(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {

            _navigationManager.NavigateTo("logout");

        }

        return response;
    }
}
