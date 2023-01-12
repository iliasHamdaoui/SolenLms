using Blazored.Toast.Services;

namespace Imanys.SolenLms.Application.WebClient.Shared.Services;

public sealed class NotificationsService
{
    private readonly IToastService _toastService;

    public NotificationsService(IToastService toastService)
    {
        _toastService = toastService;
    }

    public void ShowConfirmation(string? message)
    {
        _toastService.ShowSuccess(message, "Success");
    }

    public void ShowErreur(string? message)
    {
        _toastService.ShowError(message, "Error");
    }
}
