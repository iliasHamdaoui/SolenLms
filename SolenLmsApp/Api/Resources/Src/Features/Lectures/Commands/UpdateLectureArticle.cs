using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResources;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Imanys.SolenLms.Application.Resources.Features.Lectures.Commands.UpdateLectureArticle;

#region Web API

[Route("resources/lectures/{resourceId}")]
[ApiExplorerSettings(GroupName = CourseManagementGroupName)]
public sealed class WebApiController : BaseController
{
    /// <summary>
    /// Update a lecture article content
    /// </summary>
    /// <param name="resourceId">The id of the resource</param>
    /// <param name="command">Object containing the article content to update</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    [Authorize(Policy = CourseManagementPolicy)]
    [HttpPut("article")]
    [ProducesResponseType(typeof(RequestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RequestResponse>> UpdateContent(string resourceId,
        UpdateLectureArticleCommand? command, CancellationToken cancellationToken)
    {
        if (command == null)
            return BadRequest();

        command.ResourceId = resourceId;

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

#endregion

#region Validator

public sealed class UpdateLectureArticleCommandValidator : AbstractValidator<UpdateLectureArticleCommand>
{
    public UpdateLectureArticleCommandValidator()
    {
        RuleFor(x => x.ResourceId).NotEmpty();
        RuleFor(x => x.Content).MaximumLength(10000);
    }
}

#endregion

public sealed record UpdateLectureArticleCommand : IRequest<RequestResponse>
{
    [JsonIgnore] public string ResourceId { get; set; } = default!;
    public string? Content { get; set; }
}

internal sealed class UpdateLectureArticleCommandHandler : IRequestHandler<UpdateLectureArticleCommand, RequestResponse>
{
    #region Constructor

    private readonly IRepository<LectureResource> _repository;
    private readonly IHashids _hashids;
    private readonly IIntegrationEventsSender _eventsSender;
    private readonly ILogger<UpdateLectureArticleCommandHandler> _logger;

    public UpdateLectureArticleCommandHandler(IRepository<LectureResource> repository, IHashids hashids,
        IIntegrationEventsSender eventsSender,
        ILogger<UpdateLectureArticleCommandHandler> logger)
    {
        _repository = repository;
        _hashids = hashids;
        _eventsSender = eventsSender;
        _logger = logger;
    }

    #endregion

    public async Task<RequestResponse> Handle(UpdateLectureArticleCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeResourceId(command.ResourceId, out int resourceId))
                return Error("Invalid resource id.");

            LectureResource? resourceToUpdate = await GetResourceFromRepository(resourceId);
            if (resourceToUpdate is null)
                return Error("The resource does not exist.");

            if (ResourceContentIsNotText(resourceToUpdate))
                return Error("The lecture content format is incorrect.");

            resourceToUpdate.UpdateData(command.Content);

            await SaveResourceToRepository(resourceToUpdate, resourceId, command.ResourceId);

            await SendLectureResourceContentUpdatedEvent(command);

            return Ok("The lecture content has been updated.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while updating the lecture content.", ex);
        }
    }

    #region private methods

    private bool TryDecodeResourceId(string encodedResourceId, out int resourceId)
    {
        if (_hashids.TryDecodeSingle(encodedResourceId, out resourceId))
            return true;

        _logger.LogWarning("The encoded resource id is invalid. encodedResourceId:{encodedResourceId}",
            encodedResourceId);
        return false;
    }

    private async Task<LectureResource?> GetResourceFromRepository(int resourceId)
    {
        LectureResource? resource = await _repository.GetByIdAsync(resourceId);
        if (resource is null)
            _logger.LogWarning("The resource does not exist. resourceId:{resourceId}", resourceId);

        return resource;
    }

    private static bool ResourceContentIsNotText(LectureResource resourceToUpdate) =>
        resourceToUpdate.MediaType.Value != MediaType.Text.Value;

    private async Task SaveResourceToRepository(LectureResource resourceToUpdate, int resourceId,
        string encodedResourceId)
    {
        await _repository.UpdateAsync(resourceToUpdate);

        _logger.LogInformation(
            "Resource content updated. resourceId:{resourceId}, encodedResourceId:{encodedResourceId}", resourceId,
            encodedResourceId);
    }

    private async Task SendLectureResourceContentUpdatedEvent(UpdateLectureArticleCommand command)
    {
        LectureResourceContentUpdated contentUpdatedEvent = new()
        {
            ResourceId = command.ResourceId, Duration = ReadingTimeInSeconds(command.Content)
        };

        await _eventsSender.SendEvent(contentUpdatedEvent);
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while updating the lecture content. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private static string StripHtmlTags(string? input) =>
        input == null ? string.Empty : Regex.Replace(input, "<.*?>", String.Empty);

    private static int ReadingTimeInSeconds(string? text)
    {
        text = StripHtmlTags(text);

        if (string.IsNullOrEmpty(text))
            return 0;

        char[] punctuation = text.Where(char.IsPunctuation).Distinct().ToArray();

        decimal noOfWords = text.Split().Select(x => x.Trim(punctuation)).Count();

        decimal minutes = Math.Ceiling(noOfWords / 200);

        return (int)(minutes * 60);
    }

    #endregion
}