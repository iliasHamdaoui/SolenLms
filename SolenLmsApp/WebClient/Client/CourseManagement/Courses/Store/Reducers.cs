using Fluxor;

namespace Imanys.SolenLms.Application.WebClient.CourseManagement.Courses.Store;

public static class Reducers
{
    [ReducerMethod]
    public static CoursesState OnLoadCoursesResultAction(CoursesState state, LoadCoursesResultAction action)
    {
        return state with
        {
            Courses = action.Courses,
            CourseTotalCount = action.CourseTotalCount
        };
    }

    [ReducerMethod]
    public static CoursesState OnLoadFiltersResultAction(CoursesState state, LoadFiltersResultAction action)
    {
        return state with
        {
            Categories = action.Categories,
            Instructors = action.Instructors,
        };
    }

    [ReducerMethod]
    public static CoursesState OnSetCoursesListPageAction(CoursesState state, SetCoursesListPageAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { Page = action.Page }
        };
    }

    [ReducerMethod]
    public static CoursesState OnSetCoursesListCategoriesAction(CoursesState state, SetCoursesListCategoriesAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { CategoriesIds = action.CategoriesIds, Page = 1 }
        };
    }

    [ReducerMethod]
    public static CoursesState OnSetCoursesListInstructorsAction(CoursesState state, SetCoursesListInstructorsAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { InstructorsIds = action.InstructorsIds, Page = 1 }
        };
    }

    [ReducerMethod]
    public static CoursesState OnSetCoursesListSortedColumnAction(CoursesState state, SetCoursesListSortedColumnAction action)
    {
        return state with
        {
            GetCoursesQuery = state.GetCoursesQuery with { OrderBy = action.OrderBy, IsSortDescending = action.IsSortDescending, Page = 1 }
        };
    }

    [ReducerMethod]
    public static CoursesState OnResetCoursesListFiltersAction(CoursesState state, ResetCoursesListFiltersAction action)
    {
        return state with
        {
            GetCoursesQuery = GetAllCoursesQuery.InitialState
        };
    }
}
