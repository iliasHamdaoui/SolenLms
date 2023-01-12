using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Administration.Organization.Store;

public static class Reducers
{
    [ReducerMethod]
    public static OrganizationState OnLoadCoursesResultAction(OrganizationState state, LoadOrganizationResultAction action)
    {
        return state with
        {
            Organization = action.Organization,
        };
    }
}
