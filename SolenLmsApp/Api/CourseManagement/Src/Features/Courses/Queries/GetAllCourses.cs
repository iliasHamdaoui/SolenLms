using Imanys.SolenLms.Application.CourseManagement.Core.Domain.Courses;
using System.Linq.Expressions;

namespace Imanys.SolenLms.Application.CourseManagement.Features.Courses.Queries.GetAllCourses;

using static RequestResponse<GetAllCoursesQueryResult>;

#region Web API

[Route("course-management/courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get all the training courses
    /// </summary>
    /// <param name="query">the query to get courses</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<GetAllCoursesQueryResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAllCoursesQueryResult>> GetAllCourses([FromQuery] GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }
}

#endregion

#region Query Result

public sealed record GetAllCoursesQueryResult(List<CoursesListItem> Courses, int CourseTotalCount);

public sealed record CoursesListItem
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string? InstructorName { get; set; }
    public int Duration { get; set; }
    public bool IsPublished { get; set; }
    public DateTime LastUpdate { get; set; }
}

#endregion

public sealed record GetAllCoursesQuery : IRequest<RequestResponse<GetAllCoursesQueryResult>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = string.Empty;
    public bool IsSortDescending { get; set; } = false;
    public string CategoriesIds { get; set; } = string.Empty;
    public string InstructorsIds { get; set; } = string.Empty;
}

internal sealed class QueryHandler : IRequestHandler<GetAllCoursesQuery, RequestResponse<GetAllCoursesQueryResult>>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly IHashids _hashids;

    public QueryHandler(IRepository<Course> courseRepository, IHashids hashids)
    {
        _courseRepository = courseRepository;
        _hashids = hashids;
    }

    #endregion
    
    public async Task<RequestResponse<GetAllCoursesQueryResult>> Handle(GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        int courseTotalCount = await GetTotalCourseCountFromRepository(query, cancellationToken);

        List<Course> paginatedListOfCourses = await GetPaginatedListOfCourses(query, cancellationToken);

        List<CoursesListItem> coursesListItems = MapCoursesToCoursesListItems(paginatedListOfCourses);

        return Ok(data: new GetAllCoursesQueryResult(coursesListItems, courseTotalCount));
    }

    #region private methods

    private async Task<int> GetTotalCourseCountFromRepository(GetAllCoursesQuery query,
        CancellationToken cancellationToken)
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

#region extensions

internal static class CourseExtension
{
    public static CoursesListItem ToCourseItem(this Course course, IHashids hashids)
    {
        return new CoursesListItem
        {
            Id = hashids.Encode(course.Id),
            Title = course.Title,
            Description = course.Description,
            InstructorName = course.Instructor?.FullName,
            IsPublished = course.IsPublished,
            LastUpdate = course.LastModifiedAt,
            Duration = course.Modules.SelectMany(x => x.Lectures).Sum(x => x.Duration)
        };
    }
}

#endregion