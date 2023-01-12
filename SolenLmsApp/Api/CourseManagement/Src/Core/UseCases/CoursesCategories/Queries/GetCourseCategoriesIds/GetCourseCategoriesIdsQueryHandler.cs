using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseCategoryAggregate;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<System.Collections.Generic.List<int>>;
using Response = Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<System.Collections.Generic.List<int>>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Queries.GetCourseCategoriesIds;

internal sealed class GetCourseCategoriesIdsQueryHandler : IRequestHandler<GetCourseCategoriesIdsQuery, Response>
{
    private readonly IRepository<CourseCategory> _repository;
    private readonly IHashids _hashids;

    public GetCourseCategoriesIdsQueryHandler(IRepository<CourseCategory> repository, IHashids hashids)
    {
        _repository = repository;
        _hashids = hashids;
    }

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