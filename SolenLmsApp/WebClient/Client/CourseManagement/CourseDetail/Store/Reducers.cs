using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.CourseDetail.Store;

public static class Reducers
{


    [ReducerMethod]
    public static CourseDetailState OnLoadCourseAction(CourseDetailState state, LoadCourseAction action)
    {
        if (state.CurrentCourse != null && action.CourseId == action.CourseId)
            return state;

        return state with
        {
            CurrentCourse = null
        };
    }

    [ReducerMethod]
    public static CourseDetailState OnLoadCourseResultAction(CourseDetailState state, LoadCourseResultAction action)
    {
        return state with
        {
            CurrentCourse = action.Course
        };
    }

    [ReducerMethod]
    public static CourseDetailState OnSetCurrentModuleAction(CourseDetailState state, SetCurrentModuleAction action)
    {
        return state with
        {
            CurrentModuleId = action.ModuleId
        };
    }

    [ReducerMethod]
    public static CourseDetailState OnSetCurrentLectureAction(CourseDetailState state, SetCurrentLectureAction action)
    {
        return state with
        {
            CurrentLectureId = action.LectureId
        };
    }


    [ReducerMethod]
    public static CourseDetailState OnLoadCourseCategoriesResultAction(CourseDetailState state, LoadCourseCategoriesResultAction action)
    {
        return state with
        {
            Categories = action.Categories,
            CourseCategoriesIds = action.CourseCategoriesIds,
        };
    }

    [ReducerMethod]
    public static CourseDetailState OnLoadLearnersResultAction(CourseDetailState state, LoadLearnersResultAction action)
    {
        return state with
        {
            CourseLearners = action.Learners,
        };
    }
}
