using Fluxor.Blazor.Web.Components;

namespace Imanys.SolenLms.Application.WebClient.Shared.Components;

public abstract class ComponentWithCancellationToken : FluxorComponent, IDisposable
{
    private CancellationTokenSource? _cancellationTokenSource;

    protected CancellationToken CancellationToken => (_cancellationTokenSource ??= new()).Token;

    protected override void Dispose(bool disposed)
    {
        base.Dispose(disposed);
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}