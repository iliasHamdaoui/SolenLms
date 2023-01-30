using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using System.Linq.Expressions;

namespace Imanys.SolenLms.Application.Learning.Features.Courses.Queries.GetAllCourses;

using static RequestResponse<GetAllCoursesQueryResult>;
using Response = RequestResponse<GetAllCoursesQueryResult>;

#region Web API

[Route("courses")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ApiExplorerSettings(GroupName = LearningGroupName)]
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
    public int Duration { get; set; }
    public DateTime PublicationDate { get; set; }
    public string? InstructorName { get; set; }
    public bool IsBookmarked { get; set; }
    public DateTime? LastAccess { get; set; }
    public IEnumerable<string> Categories { get; set; } = default!;
    public float LearnerProgress { get; set; }
}

#endregion

public sealed record GetAllCoursesQuery : IRequest<RequestResponse<GetAllCoursesQueryResult>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = string.Empty;
    public string CategoriesIds { get; set; } = string.Empty;
    public string ReferentsIds { get; set; } = string.Empty;
    public bool BookmarkedOnly { get; set; } = false;
}

internal sealed class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, Response>
{
    #region Constructor

    private readonly IRepository<Course> _courseRepository;
    private readonly ICurrentUser _currentUser;
    
    public GetAllCoursesQueryHandler(IRepository<Course> courseRepository, ICurrentUser currentUser)
    {
        _courseRepository = courseRepository;
        _currentUser = currentUser;
    }

    #endregion
    
    public async Task<Response> Handle(GetAllCoursesQuery query, CancellationToken cancellationToken)
    {
        int courseTotalCount = await GetTotalCourseCountFromRepository(query, cancellationToken);

        List<Course> courses = await GetPaginatedListOfCourses(query, cancellationToken);

        List<CoursesListItem> coursesListItems = courses.Select(x => x.ToListItem()).ToList();

        return Ok(data: new GetAllCoursesQueryResult(coursesListItems, courseTotalCount));
    }

    #region private methods

    private async Task<int> GetTotalCourseCountFromRepository(GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        return await _courseRepository.CountAsync(new GetAllCoursesSpec(_currentUser.UserId, query), cancellationToken);
    }

    private async Task<List<Course>> GetPaginatedListOfCourses(GetAllCoursesQuery query,
        CancellationToken cancellationToken)
    {
        return await _courseRepository.ListAsync(new GetAllCoursesPagingSpec(_currentUser.UserId, query),
            cancellationToken);
    }

    private class GetAllCoursesSpec : Specification<Course>
    {
        public GetAllCoursesSpec(string learnerId, GetAllCoursesQuery query)
        {
            Query
                .Where(x => x.IsPublished)
                .Include(course => course.Instructor)
                .Include(course => course.Categories)
                .ThenInclude(category => category.Category)
                .Include(course => course.LearnersBookmarks.Where(x => x.LearnerId == learnerId))
                .Include(course => course.LearnersProgress.Where(x => x.LearnerId == learnerId))
                .AsSplitQuery()
                .AsNoTracking();

            Dictionary<string, Expression<Func<Course, object>>> columnsMap = new()
            {
                ["title"] = x => x.Title,
                ["titleDesc"] = x => x.Title,
                ["publicationDate"] = c => c.PublicationDate,
                ["publicationDateDesc"] = c => c.PublicationDate,
                ["instructor"] = c => c.Instructor!.FullName,
                ["instructorDesc"] = c => c.Instructor!.FullName,
            };


            if (!string.IsNullOrEmpty(query.OrderBy) && columnsMap.ContainsKey(query.OrderBy))
            {
                if (query.OrderBy.EndsWith("Desc"))
                    Query.OrderByDescending(columnsMap[query.OrderBy]!);
                else
                    Query.OrderBy(columnsMap[query.OrderBy]!);
            }
            else
            {
                Query.OrderByDescending(x => x.LearnersProgress.FirstOrDefault()!.LastAccessTime);
            }

            if (!string.IsNullOrEmpty(query.CategoriesIds))
            {
                var categoriesIds = query.CategoriesIds.Split(',');
                if (categoriesIds.Any() && categoriesIds.All(x => !string.IsNullOrWhiteSpace(x)))
                    Query.Where(x => x.Categories.Any(c => categoriesIds.Contains(c.CategoryId.ToString())));
            }

            if (!string.IsNullOrEmpty(query.ReferentsIds))
            {
                var referentsIds = query.ReferentsIds.Split(',');
                if (referentsIds.Any() && referentsIds.All(x => !string.IsNullOrWhiteSpace(x)))
                    Query.Where(x => x.InstructorId != null && referentsIds.Contains(x.InstructorId));
            }

            if (query.BookmarkedOnly)
            {
                Query.Where(x => x.LearnersBookmarks.Any(book => book.LearnerId == learnerId));
            }
        }
    }

    private sealed class GetAllCoursesPagingSpec : GetAllCoursesSpec
    {
        public GetAllCoursesPagingSpec(string learnerId, GetAllCoursesQuery query) : base(learnerId, query)
        {
            Query.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize);
        }
    }

    #endregion
}

#region extensions

internal static class CourseExtension
{
    public static CoursesListItem ToListItem(this Course course)
    {
        return new CoursesListItem
        {
            Id = course.Id,
            Title = course.Title,
            Description = course.Description,
            Duration = course.Duration,
            InstructorName = course.Instructor?.FullName,
            PublicationDate = course.PublicationDate,
            LastAccess = course.LearnersProgress?.FirstOrDefault()?.LastAccessTime,
            LearnerProgress = course.LearnersProgress?.FirstOrDefault()?.Progress ?? 0,
            IsBookmarked = course.LearnersBookmarks.Any(),
            Categories = course.Categories.Select(x => x.Category.Name)
        };
    }
}

#endregion