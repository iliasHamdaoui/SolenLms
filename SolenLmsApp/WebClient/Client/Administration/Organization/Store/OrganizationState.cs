using Fluxor;
using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Administration.Organization.Store;

public sealed record OrganizationState
{
    public GetOrganizationQueryResult? Organization { get; set; }
}


public sealed class OrganizationFeatureState : Feature<OrganizationState>
{
    public override string GetName() => nameof(OrganizationFeatureState);

    protected override OrganizationState GetInitialState()
    {
        return new OrganizationState();
    }
}