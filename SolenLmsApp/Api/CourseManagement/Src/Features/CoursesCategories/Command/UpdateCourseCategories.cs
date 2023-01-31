using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CoursesCategories;
using System.Text.Json.Serialization;

namespace Imanys.SolenLms.Application.CourseManagement.Features.CoursesCategories.Command.UpdateCourseCategories;

#region Web API

[Route("course-management/courses/{courseId}/categories")]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[Authorize(Policy = CourseManagementPolicy)]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// update a course categories
    /// </summary>
    /// <param name="courseId">The id of the training course</param>
    /// <param name="command">Object containing the categories to add to the course</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns> an ActionResult type of RequestResponse</returns>
    [HttpPut]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateCourseCategories(string courseId,
        UpdateCourseCategoriesCommand? command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.CourseId = courseId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class UpdateCourseCategoriesCommandValidator : AbstractValidator<UpdateCourseCategoriesCommand>
{
    public UpdateCourseCategoriesCommandValidator()
    {
        RuleFor(x => x.SelectecdCategroriesIds).NotNull();
    }
}

#endregion

public sealed record UpdateCourseCategoriesCommand : IRequest<RequestResponse>
{
    public List<int> SelectecdCategroriesIds { get; set; } = default!;
    [JsonIgnore] public string CourseId { get; set; } = default!;
}

internal sealed class CommandHandler : IRequestHandler<UpdateCourseCategoriesCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<CourseCategory> _courseCategoryRepo;
    private readonly IHashids _hashids;
    private readonly ILogger<CommandHandler> _logger;

    public CommandHandler(IRepository<CourseCategory> courseCategoryRepo, IHashids hashids,
        ILogger<CommandHandler> logger)
    {
        _courseCategoryRepo = courseCategoryRepo;
        _hashids = hashids;
        _logger = logger;
    }

    #endregion
    
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