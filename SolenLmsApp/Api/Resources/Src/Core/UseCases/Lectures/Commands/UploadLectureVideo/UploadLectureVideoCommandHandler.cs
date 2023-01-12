﻿using Imanys.SolenLms.Application.Resources.Core.Domain.LectureResourceAggregate;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.Resources;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;

internal sealed class UploadLectureVideoCommandHandler : IRequestHandler<UploadLectureVideoCommand, RequestResponse>
{
    #region constructor

    private readonly IRepository<LectureResource> _repository;
    private readonly IMediaManager _videoManager;
    private readonly IHashids _hashids;
    private readonly ICurrentUser _currentUser;
    private readonly IIntegratedEventsSender _eventsSender;
    private readonly IStorageRepo _storageRepo;
    private readonly int _maxMediaStorageSpaceInGb;
    private readonly ILogger<UploadLectureVideoCommandHandler> _logger;

    public UploadLectureVideoCommandHandler(IRepository<LectureResource> repository, IMediaManager videoManager,
        IHashids hashids, ICurrentUser currentUser, IIntegratedEventsSender eventsSender, IStorageRepo storageRepo,
        IOptions<ResourcesSettings> resourceOptions, ILogger<UploadLectureVideoCommandHandler> logger)
    {
        _repository = repository;
        _videoManager = videoManager;
        _hashids = hashids;
        _currentUser = currentUser;
        _eventsSender = eventsSender;
        _storageRepo = storageRepo;
        _maxMediaStorageSpaceInGb = resourceOptions.Value.MaxMediaStorageSpaceInGb;
        _logger = logger;
    }

    #endregion

    public async Task<RequestResponse> Handle(UploadLectureVideoCommand command, CancellationToken _)
    {
        try
        {
            // processing validation
            if (!TryDecodeResourceId(command.ResourceId, out int resourceId))
                return Error("Invalid resource id.");
            
            if (await CheckIfUserHasExceededMaximumStorage(command.File))
                return Error($"You've reached the maximum storage available ({_maxMediaStorageSpaceInGb} Gb)");

            LectureResource? resourceToUpdate = await GetResourceFromRepository(resourceId);
            if (resourceToUpdate is null)
                return Error("The resource does not exist.");

            if (FileHasNotTheCorrectFormat(resourceToUpdate, command.File))
                return Error("The video format is incorrect.");
            
            // uploading the video
            MediaUploadResult uploadResult = await UploadVideo(resourceToUpdate, command.File);
            if (!uploadResult.IsSuccess)
                return Error("Error occured while uploading the video.");

            // saving data
            resourceToUpdate.UpdateData(uploadResult.MediaName, command.File.Length);
            await SaveResourceToRepository(resourceToUpdate, resourceId, command.ResourceId);

            // sending event
            await SendLectureResourceContentUpdatedEvent(command.ResourceId, uploadResult);

            return Ok("The video has been uploaded.");
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while uploading the video.", ex);
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

    private static bool FileHasNotTheCorrectFormat(LectureResource resourceToUpdate, IResourceFile file) =>
        !string.Equals(resourceToUpdate.MediaType.Value, file.ContentType, StringComparison.CurrentCultureIgnoreCase);

    private async Task<bool> CheckIfUserHasExceededMaximumStorage(IResourceFile file)
    {
        if (_maxMediaStorageSpaceInGb == 0) // 0 == unlimited storage
            return false;

        long maxStorage = _maxMediaStorageSpaceInGb * 1024L * 1024L * 1024L; // convert GB to bytes 
        long currentStorage = await _storageRepo.GetCurrentStorageRepo(_currentUser.OrganizationId, default);

        return currentStorage + file.Length > maxStorage;
    }

    private async Task<MediaUploadResult> UploadVideo(LectureResource resourceToUpdate, IResourceFile file)
    {
        return await _videoManager.Upload(file, _currentUser.OrganizationId,
            resourceToUpdate.CourseId, resourceToUpdate.ModuleId, resourceToUpdate.LectureId);
    }

    private async Task SaveResourceToRepository(LectureResource resourceToUpdate, int resourceId,
        string encodedResourceId)
    {
        await _repository.UpdateAsync(resourceToUpdate);

        _logger.LogInformation(
            "Resource content updated. resourceId:{resourceId}, encodedResourceId:{encodedResourceId}", resourceId,
            encodedResourceId);
    }

    private async Task SendLectureResourceContentUpdatedEvent(string resourceId, MediaUploadResult uploadResult)
    {
        LectureResourceContentUpdated contentUpdatedEvent = new()
        {
            ResourceId = resourceId, Duration = uploadResult.Duration
        };

        await _eventsSender.SendEvent(contentUpdatedEvent);
    }

    private RequestResponse UnexpectedError(string error, Exception ex)
    {
        _logger.LogError(ex, "Error occured while uploading the video. message:{message}", ex.Message);
        return Error(ResponseError.Unexpected, error);
    }

    #endregion
}