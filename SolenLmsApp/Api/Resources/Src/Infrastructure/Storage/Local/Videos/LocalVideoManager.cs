using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Storage.Local.Videos;
internal sealed class LocalVideoManager : IMediaManager
{
    private readonly LocalStorageSettings _settings;
    private readonly ILogger<LocalVideoManager> _logger;
    private readonly IWebHostEnvironment _env;


    public LocalVideoManager(IOptions<LocalStorageSettings> settings, ILogger<LocalVideoManager> logger, IWebHostEnvironment env)
    {
        _settings = settings.Value;
        _logger = logger;
        _env = env;
    }

    public Task DeleteCourseMedias(string organizationId, string courseId)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), GetCourseFolder(organizationId, courseId));

        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        return Task.CompletedTask;
    }

    public Task DeleteLectureMedias(string organizationId, string courseId, string moduleId, string lectureId)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), GetLectureFolder(organizationId, courseId, moduleId, lectureId));

        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        return Task.CompletedTask;
    }

    public Task DeleteModuleMedias(string organizationId, string courseId, string moduleId)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), GetModuleFolder(organizationId, courseId, moduleId));

        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        return Task.CompletedTask;
    }

    public Task DeleteOrganizationMedias(string organizationId)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), GetOrganizationFolder(organizationId));

        if (Directory.Exists(directory))
            Directory.Delete(directory, true);

        return Task.CompletedTask;
    }

    public Task<byte[]> GetMediaContent(string? mediaPath)
    {
        if (mediaPath == null)
            return Task.FromResult(Array.Empty<byte>());

        mediaPath = Path.Combine(Directory.GetCurrentDirectory(), mediaPath);

        if (File.Exists(mediaPath))
            return Task.FromResult(File.ReadAllBytes(mediaPath));

        return Task.FromResult(Array.Empty<byte>());
    }

    public async Task<MediaUploadResult> Upload(IResourceFile media, string organizationId, string courseId, string moduleId, string lectureId)
    {
        var result = new MediaUploadResult { IsSuccess = true };
        try
        {
        
            var videoName = $"{Guid.NewGuid()}{media.FileExtension}";

            var lectureFolder = GetLectureFolder(organizationId, courseId, moduleId, lectureId);

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), lectureFolder)))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), lectureFolder));

            result.MediaName = Path.Combine(lectureFolder, videoName);

            var videoPath = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine(lectureFolder, videoName));

            using var stream = new FileStream(videoPath, FileMode.Create);
            await media.CopyToAsync(stream);

            result.Duration = await Helpers.GetVideoDuration(_env, videoPath, _logger);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.Error = "Error occured while uploading media";
            _logger.LogError(ex, "Error occured while uploading media, {message}", ex.Message);
        }

        return result;
    }


    private string GetLectureFolder(string organizationId, string courseId, string moduleId, string lectureId) => Path.Combine(GetModuleFolder(organizationId, courseId, moduleId), lectureId);

    private string GetModuleFolder(string organizationId, string courseId, string moduleId) => Path.Combine(GetCourseFolder(organizationId, courseId), moduleId);

    private string GetCourseFolder(string organizationId, string courseId) => Path.Combine(GetOrganizationFolder(organizationId), courseId);

    private string GetOrganizationFolder(string organizationId) => Path.Combine(_settings.ResourcesFolder, organizationId);

    public async Task<Stream?> GetMediaContentStream(string? mediaPath)
    {

        if (mediaPath == null)
            return null;

        mediaPath = Path.Combine(Directory.GetCurrentDirectory(), mediaPath);

        if (File.Exists(mediaPath))
        {
            var ms = new MemoryStream();
            var fs = new FileStream(mediaPath, FileMode.Open, FileAccess.Read);
            await fs.CopyToAsync(ms);
        }
        
        return null;
    }
}
