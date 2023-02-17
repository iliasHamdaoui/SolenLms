using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Xabe.FFmpeg;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.Storage;

internal static class Helpers
{
    public static async Task<int> GetVideoDuration(IWebHostEnvironment env, string videoPath, ILogger logger)
    {
        try
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            // FFmpeg.SetExecutablesPath(Path.Combine(env.ContentRootPath, "ffmpeg"));

            IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
            var videoDuration = Convert.ToInt32(mediaInfo.VideoStreams.First().Duration.TotalSeconds);

            stopwatch.Stop();
            logger.LogInformation("Video duration calculated in : {elapsed} ms", stopwatch.ElapsedMilliseconds);

            return videoDuration;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while calculation the duration, {videoPath}, {message}", videoPath,
                ex.Message);

            return 0;
        }
    }
}