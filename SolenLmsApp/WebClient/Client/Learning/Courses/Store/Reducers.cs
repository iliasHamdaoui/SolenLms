using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.Learning.Courses.Store;

public static class Reducers
{

    [ReducerMethod]
    public static LearningCoursesState OnLoadCoursesResultAction(LearningCoursesState state, LoadCoursesResultAction action)
    {
        return state with
        {
            Courses = action.Courses,
            CourseTotalCount = action.CourseTotalCount
        };
    }


    [ReducerMethod]
    public static LearningCoursesState OnLoadCoursesFiltersResultAction(LearningCoursesState state, LoadCoursesFiltersResultAction action)
    {
        return state with
        {
            Categories = action.Categories,
            Instructors = action.Instructors,
        };
    }

    [ReducerMethod]
    public static LearningCoursesState OnSetCoursesListPageAction(LearningCoursesState state, SetCoursesListPageAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { Page = action.Page }
        };
    }

    [ReducerMethod]
    public static LearningCoursesState OnSetCoursesListCategoriesAction(LearningCoursesState state, SetCoursesListCategoriesAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { CategoriesIds = action.CategoriesIds, Page = 1 }
        };
    }

    [ReducerMethod]
    public static LearningCoursesState OnSetCoursesListInstructorsAction(LearningCoursesState state, SetCoursesListInstructorsAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { InstructorsIds = action.InstructorssIds, Page = 1 }
        };
    }

    [ReducerMethod]
    public static LearningCoursesState OnSetCoursesListSortedColumnAction(LearningCoursesState state, SetCoursesListSortedByAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { OrderBy = action.OrderBy, Page = 1 }
        };
    }

    [ReducerMethod]
    public static LearningCoursesState OnSetCoursesListBookmarkOnlyAction(LearningCoursesState state, SetCoursesListBookmarkOnlyAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { BookmarkOnly = action.BookmarkOnly }
        };
    }

    [ReducerMethod]
    public static LearningCoursesState OnResetCoursesListFiltersAction(LearningCoursesState state, ResetCoursesListFiltersAction action)
    {
        return state with
        {
            GetCoursesQuery = GetAllCoursesQuery.InitialState
        };
    }
}
