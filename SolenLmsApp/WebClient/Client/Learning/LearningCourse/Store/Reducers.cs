using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Learning.LearningCourse.Store;

public static class Reducers
{
    [ReducerMethod]
    public static LearningState OnLoadLearningCourseAction(LearningState state, LoadLearningCourseAction action)
    {
        if (state.Course != null && state.Course.CourseId == action.CourseId)
            return state;

        return state with
        {
            Course = null
        };
    }

    [ReducerMethod]
    public static LearningState OnLoadLearningCourseResultAction(LearningState state, LoadLearningCourseResultAction action)
    {
        return state with
        {
            Course = action.Course
        };
    }

    [ReducerMethod]
    public static LearningState OnLoadLectureAction(LearningState state, LoadLectureAction action)
    {
        return state with
        {
            CurrentLecture = null
        };
    }

    [ReducerMethod]
    public static LearningState OnLoadLectureResultAction(LearningState state, LoadLectureResultAction action)
    {
        return state with
        {
            CurrentLecture = action.Lecture
        };
    }
}
