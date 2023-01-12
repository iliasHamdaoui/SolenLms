using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using System.Linq.Expressions;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.
    CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses.GetAllCoursesQueryResult>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Queries.GetAllCourses;

internal sealed class
    GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, RequestResponse<GetAllCoursesQueryResult>>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;

    public GetAllCoursesQueryHandler(IRepository<Course> courseRepository, IHashids hashids)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
    }

    public async Task<RequestResponse<GetAllCoursesQueryResult>> Handle(GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        int courseTotalCount = await GetTotalCourseCountFromRepository(query, cancellationToken);

        List<Course> paginatedListOfCourses = await GetPaginatedListOfCourses(query, cancellationToken);

        List<CoursesListItem> coursesListItems = MapCoursesToCoursesListItems(paginatedListOfCourses);

        return Ok(data: new GetAllCoursesQueryResult(coursesListItems, courseTotalCount));
    }

    #region private methods

    private async Task<int> GetTotalCourseCountFromRepository(GetAllCoursesQuery query, CancellationToken cancellationToken)
    {
        return await _courseRepository.CountAsync(new GetAllCoursesSpec(query), cancellationToken);
    }

    private async Task<List<Course>> GetPaginatedListOfCourses(GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        return await _courseRepository.ListAsync(new GetAllCoursesPagingSpec(query), cancellationToken);
    }

    private List<CoursesListItem> MapCoursesToCoursesListItems(IEnumerable<Course> courses)
    {
        return courses.Select(x => x.ToCourseItem(_hashids)).ToList();
    }

    private class GetAllCoursesSpec : Specification<Course>
    {
        public GetAllCoursesSpec(GetAllCoursesQuery query)
        {
            Query
                .Include(course => course.Modules)
                .ThenInclude(module => module.Lectures)
                .Include(course => course.Instructor)
                .Include(course => course.Categories)
                .AsSplitQuery()
                .AsNoTracking();

            Dictionary<string, Expression<Func<Course, object>>> columnsMap = new()
            {
                ["courseTitle"] = x => x.Title,
                ["lastUpdate"] = x => x.LastModifiedAt,
                ["instructor"] = c => c.Instructor!.FamilyName
            };


            if (!string.IsNullOrEmpty(query.OrderBy) && columnsMap.ContainsKey(query.OrderBy))
            {
                if (query.IsSortDescending)
                    Query.OrderByDescending(columnsMap[query.OrderBy]!);
                else
                    Query.OrderBy(columnsMap[query.OrderBy]!);
            }

            if (!string.IsNullOrEmpty(query.CategoriesIds))
            {
                var categoriesIds = query.CategoriesIds.Split(',');
                if (categoriesIds.Any())
                    Query.Where(x => x.Categories.Any(c => categoriesIds.Contains(c.CategoryId.ToString())));
            }

            if (!string.IsNullOrEmpty(query.InstructorsIds))
            {
                var instructorsIds = query.InstructorsIds.Split(',');
                if (instructorsIds.Any())
                    Query.Where(x => instructorsIds.Contains(x.InstructorId));
            }
        }
    }

    private sealed class GetAllCoursesPagingSpec : GetAllCoursesSpec
    {
        public GetAllCoursesPagingSpec(GetAllCoursesQuery query) : base(query)
        {
            Query.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
        }
    }

    #endregion
}