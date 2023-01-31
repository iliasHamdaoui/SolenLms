using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CoursesCategories;

namespace Imanys.SolenLms.Application.CourseManagement.Features.CoursesCategories.Queries.GetCourseCategoriesIds;

using static RequestResponse<List<int>>;
using Response = RequestResponse<List<int>>;

#region Web API

[Route("course-management/courses/{courseId}/categories")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Get course categories ids
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(RequestResponse<List<int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResponse<List<int>>>> GetCourseCategoriesIds(string courseId,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCourseCategoriesIdsQuery(courseId), cancellationToken));
    }
}

#endregion

public sealed record GetCourseCategoriesIdsQuery(string CourseId) : IRequest<RequestResponse<List<int>>>;

internal sealed class GetCourseCategoriesIdsQueryHandler : IRequestHandler<GetCourseCategoriesIdsQuery, Response>
{
    #region Constructor

    private readonly IRepository<CourseCategory> _repository;
    private readonly IHashids _hashids;

    public GetCourseCategoriesIdsQueryHandler(IRepository<CourseCategory> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

    #endregion
    
    public async Task<Response> Handle(GetCourseCategoriesIdsQuery query, CancellationToken cancellationToken)
    {
        if (!TryDecodeCourseId(query.CourseId, out int courseId))
            return NotFound("The course id is invalid.");

        List<CourseCategory> courseCategories = await GetCourseCategoriesFromRepository(cancellationToken, courseId);

        List<int> categoriesIds = courseCategories.Select(x => x.CategoryId).ToList();

        return Ok(data: categoriesIds);
    }
    
    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId) =>
        _hashids.TryDecodeSingle(encodedCourseId, out courseId);

    private async Task<List<CourseCategory>> GetCourseCategoriesFromRepository(CancellationToken cancellationToken,
        int courseId)
    {
        return await _repository.ListAsync(new GetCourseCategoriesSpec(courseId), cancellationToken);
    }

    private sealed class GetCourseCategoriesSpec : Specification<CourseCategory>
    {
        public GetCourseCategoriesSpec(int courseId)
        {
            Query.Where(x => x.CourseId == courseId)
                .AsNoTracking();
        }
    }

    #endregion
}