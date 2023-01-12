using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Imanys.SolenLms.Application.Shared.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Storage.AzureBlobStorage;

internal sealed class AzureBobVideoManager : IMediaManager
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ICurrentUser _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<AzureBobVideoManager> _logger;
    private readonly IOptions<AzureBlobStorageSettings> _azureOptions;

    public AzureBobVideoManager(BlobServiceClient blobServiceClient, ICurrentUser currentUser, IWebHostEnvironment env,
        ILogger<AzureBobVideoManager> logger, IOptions<AzureBlobStorageSettings> azureOptions)
    {
        _blobServiceClient = blobServiceClient;
        _currentUser = currentUser;
        _env = env;
        _logger = logger;
        _azureOptions = azureOptions;
    }

    public async Task DeleteCourseMedias(string organizationId, string courseId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(organizationId);

        var allCourseBlobs = containerClient.GetBlobsAsync(prefix: courseId);

        await foreach (var blob in allCourseBlobs)
        {
            await containerClient.DeleteBlobAsync(blob.Name);
        }
    }

    public async Task DeleteLectureMedias(string organizationId, string courseId, string moduleId, string lectureId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(organizationId);

        var allLecureblobs = containerClient.GetBlobsAsync(prefix: $"{courseId}/{moduleId}/{lectureId}");

        await foreach (var blob in allLecureblobs)
        {
            await containerClient.DeleteBlobAsync(blob.Name);
        }
    }

    public async Task DeleteModuleMedias(string organizationId, string courseId, string moduleId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(organizationId);

        var allModuleBlobs = containerClient.GetBlobsAsync(prefix: $"{courseId}/{moduleId}");

        await foreach (var blob in allModuleBlobs)
        {
            await containerClient.DeleteBlobAsync(blob.Name);
        }
    }

    public async Task DeleteOrganizationMedias(string organizationId)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(organizationId);
        await containerClient.DeleteIfExistsAsync();
    }

    public async Task<byte[]> GetMediaContent(string? mediaPath)
    {
        if (mediaPath == null)
            return Array.Empty<byte>();


        var containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.OrganizationId);
        var blobClient = containerClient.GetBlobClient(mediaPath);
        if (await blobClient.ExistsAsync())
        {
            using var ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            return ms.ToArray();
        }


        return Array.Empty<byte>();
    }

    public async Task<Stream?> GetMediaContentStream(string? mediaPath)
    {
        if (mediaPath == null)
            return null;

        var containerClient = _blobServiceClient.GetBlobContainerClient(_currentUser.OrganizationId);
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
        var fileTempPath = Path.Combine(GetTempFolder(), fileName);
        var stream = new FileStream(fileTempPath, FileMode.Create);
        try
        {
            await resourceFile.CopyToAsync(stream);
        }
        finally
        {
            await stream.DisposeAsync();
        }

        // Todo: calculate duration in azure function   
        result.Duration = await Helpers.GetVideoDuration(_env, fileTempPath, _logger);

        File.Delete(fileTempPath);

        var containerClient = _blobServiceClient.GetBlobContainerClient(organizationId);

        await containerClient.CreateIfNotExistsAsync();

        var fileBlobName = $"{courseId}/{moduleId}/{lectureId}/{fileName}";

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

    private string GetTempFolder()
    {
        var tempDirectory = Path.Combine(Directory.GetCurrentDirectory(), _azureOptions.Value.TempDirectory);

        if (!Directory.Exists(tempDirectory))
            Directory.CreateDirectory(tempDirectory);

        return tempDirectory;
    }
}