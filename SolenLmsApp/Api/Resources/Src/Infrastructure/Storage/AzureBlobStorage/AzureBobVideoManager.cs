using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Imanys.SolenLms.Application.Resources.Features;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Storage.AzureBlobStorage;

internal sealed class AzureBobVideoManager : IMediaManager
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly AzureBlobStorageSettings _blobStorageSettings;

    public AzureBobVideoManager(BlobServiceClient blobServiceClient, IOptions<AzureBlobStorageSettings> azureOptions)
    {
        _blobServiceClient = blobServiceClient;
        _blobStorageSettings = azureOptions.Value;
    }

    public async Task DeleteCourseMedias(string organizationId, string courseId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ResourcesContainerName);

        var allCourseBlobs = containerClient.GetBlobsAsync(prefix: $"{organizationId}/{courseId}");

        await foreach (var blob in allCourseBlobs)
            await containerClient.DeleteBlobAsync(blob.Name);
    }

    public async Task DeleteLectureMedias(string organizationId, string courseId, string moduleId, string lectureId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ResourcesContainerName);

        var allLectureBlobs = containerClient.GetBlobsAsync(prefix: $"{organizationId}/{courseId}/{moduleId}/{lectureId}");

        await foreach (var blob in allLectureBlobs)
            await containerClient.DeleteBlobAsync(blob.Name);
    }

    public async Task DeleteModuleMedias(string organizationId, string courseId, string moduleId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ResourcesContainerName);

        var allModuleBlobs = containerClient.GetBlobsAsync(prefix: $"{organizationId}/{courseId}/{moduleId}");

        await foreach (var blob in allModuleBlobs)
            await containerClient.DeleteBlobAsync(blob.Name);
    }

    public async Task DeleteOrganizationMedias(string organizationId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ResourcesContainerName);
        var allOrganizationBlobs = containerClient.GetBlobsAsync(prefix: organizationId);

        await foreach (var blob in allOrganizationBlobs)
            await containerClient.DeleteBlobAsync(blob.Name);
    }
    
    public async Task<Stream?> GetMediaContentStream(string? mediaPath)
    {
        if (mediaPath == null)
            return null;

        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ResourcesContainerName);
        var blobClient = containerClient.GetBlobClient(mediaPath);
        if (await blobClient.ExistsAsync())
        {
            return await blobClient.OpenReadAsync();
        }

        return null;
    }

    public async Task<MediaUploadResult> Upload(IResourceFile resourceFile, string organizationId, string courseId,
        string moduleId, string lectureId)
    {
        var result = new MediaUploadResult { IsSuccess = true };

        var fileName = $"{Guid.NewGuid()}{resourceFile.FileExtension}";

        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobStorageSettings.ResourcesContainerName);

        await containerClient.CreateIfNotExistsAsync();

        var fileBlobName = $"{organizationId}/{courseId}/{moduleId}/{lectureId}/{fileName}";

        var blobClient = containerClient.GetBlobClient(fileBlobName);

        var httpHeaders = new BlobHttpHeaders { ContentType = resourceFile.ContentType };

        var res = await blobClient.UploadAsync(resourceFile.OpenReadStream(), httpHeaders);

        if (res == null)
        {
            result.IsSuccess = false;
            return result;
        }

        result.MediaName = fileBlobName;

        return result;
    }
    
}