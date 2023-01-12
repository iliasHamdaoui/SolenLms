using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Administration.Users.Store;

public static class Reducers
{
    [ReducerMethod]
    public static UsersState OnLoadCoursesResultAction(UsersState state, LoadUsersResultAction action)
    {
        return state with
        {
            Users = action.QueryResult.Users,
        };
    }
}
