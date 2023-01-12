using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using System.Linq.Expressions;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.Learning.Core.
    UseCases.Courses.Queries.GetAllCourses.GetAllCoursesQueryResult>;
using Response =
    Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<Imanys.SolenLms.Application.Learning.Core.UseCases.
        Courses.Queries.GetAllCourses.GetAllCoursesQueryResult>;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetAllCourses;

internal sealed class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, Response>
{
    private readonly IRepository<Course> _courseRepository;
    private readonly ICurrentUser _currentUser;


    public GetAllCoursesQueryHandler(IRepository<Course> courseRepository, ICurrentUser currentUser)
    {
        _courseRepository = courseRepository;
        _currentUser = currentUser;
    }

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