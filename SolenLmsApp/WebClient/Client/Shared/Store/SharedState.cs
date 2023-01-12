using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Shared.Store;

public sealed record SharedState
{
    public string OrganizationName { get; set; } = string.Empty;
}


public sealed class SharedFeatureState : Feature<SharedState>
{
    public override string GetName() => nameof(SharedFeatureState);

    protected override SharedState GetInitialState()
    {
        return new SharedState();
    }
}