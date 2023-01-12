using Fluxor;
using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Administration.Users.Store;

public sealed record UsersState
{
    public ICollection<UserForGetUsersQueryResult>? Users { get; set; }
}


public sealed class UsersFeatureState : Feature<UsersState>
{
    public override string GetName() => nameof(UsersFeatureState);

    protected override UsersState GetInitialState()
    {
        return new UsersState();
    }
}