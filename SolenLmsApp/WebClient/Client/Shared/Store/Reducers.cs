using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Shared.Store;

public static class Reducers
{
    [ReducerMethod]
    public static SharedState OnLoadCoursesResultAction(SharedState state, LoadOrganizationNameResultAction action)
    {
        return state with
        {
            OrganizationName = action.OrganizationName,
        };
    }
}
