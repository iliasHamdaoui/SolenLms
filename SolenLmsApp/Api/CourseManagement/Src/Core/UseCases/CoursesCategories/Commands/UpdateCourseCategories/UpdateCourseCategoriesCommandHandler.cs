using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseCategoryAggregate;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.CoursesCategories.Commands.UpdateCourseCategories;

internal sealed class
    UpdateCourseCategoriesCommandHandler : IRequestHandler<UpdateCourseCategoriesCommand, RequestResponse>
{
    private readonly IRepository<CourseCategory> _courseCategoryRepo;
    private readonly IHashids _hashids;
    private readonly ILogger<UpdateCourseCategoriesCommandHandler> _logger;

    public UpdateCourseCategoriesCommandHandler(IRepository<CourseCategory> courseCategoryRepo, IHashids hashids,
        ILogger<UpdateCourseCategoriesCommandHandler> logger)
    {
        _courseCategoryRepo = courseCategoryRepo;
        _hashids = hashids;
        _logger = logger;
    }

    public async Task<RequestResponse> Handle(UpdateCourseCategoriesCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            List<CourseCategory> existingCategories = await GetActualCourseCategoriesFromRepository(courseId);

            IEnumerable<CourseCategory> categoriesToAdd = GetCategoriesToAdd(command, existingCategories, courseId);
            await AddNewCategoriesToRepository(categoriesToAdd);

            List<CourseCategory> categoriesToRemove = GetCategoriesToRemove(command, existingCategories);
            await DeleteCategoriesToRemoveFromRepository(categoriesToRemove);

            return Ok("The course categories have been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating course categories.", ex);
        }
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId)
    {
        if (_hashids.TryDecodeSingle(encodedCourseId, out courseId))
            return true;

        _logger.LogWarning("The encoded course id is invalid. encodedCourseId:{encodedCourseId}", encodedCourseId);
        return false;
    }

    private async Task<List<CourseCategory>> GetActualCourseCategoriesFromRepository(int courseId)
    {
        return await _courseCategoryRepo.ListAsync(new GetCourseCategoriesSpec(courseId));
    }

    private static IEnumerable<CourseCategory> GetCategoriesToAdd(UpdateCourseCategoriesCommand command,
        IEnumerable<CourseCategory> existingCourseCategories, int courseId)
    {
        return command.SelectecdCategroriesIds
            .Where(categoryId => existingCourseCategories.All(x => x.CategoryId != categoryId))
            .Select(categoryId => new CourseCategory(courseId, categoryId))
            .ToList();
    }

    private async Task AddNewCategoriesToRepository(IEnumerable<CourseCategory> categoriesToAdd)
    {
        await _courseCategoryRepo.AddRangeAsync(categoriesToAdd);
    }


    private static List<CourseCategory> GetCategoriesToRemove(UpdateCourseCategoriesCommand command,
        IEnumerable<CourseCategory> existingCourseCategories)
    {
        return existingCourseCategories
            .Where(x => command.SelectecdCategroriesIds.All(c => c != x.CategoryId))
            .ToList();
    }

    private async Task DeleteCategoriesToRemoveFromRepository(List<CourseCategory> categoriesToRemove)
    {
        await _courseCategoryRepo.DeleteRangeAsync(categoriesToRemove);
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while updating course categories. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private sealed class GetCourseCategoriesSpec : Specification<CourseCategory>
    {
        public GetCourseCategoriesSpec(int courseId)
        {
            Query.Where(x => x.CourseId == courseId);
        }
    }

    #endregion
}