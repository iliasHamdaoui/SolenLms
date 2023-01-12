using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Learning.CourseOverview.Store;

public static class Reducers
{

    [ReducerMethod]
    public static LearningCourseOverviewState OnLoadCourseAction(LearningCourseOverviewState state, LoadCourseAction action)
    {
        return state with
        {
            CurrentCourse = null
        };
    }

    [ReducerMethod]
    public static LearningCourseOverviewState OnLoadCourseResultAction(LearningCourseOverviewState state, LoadCourseResultAction action)
    {
        return state with
        {
            CurrentCourse = action.Course
        };
    }
}
