namespace Imanys.SolenLms.Application.Resources.Core.UseCases;
internal sealed class MediaUploadResult
{
    public required bool IsSuccess { get; set; }
    public string? MediaName { get; set; }
    public string? Error { get; set; }
    public int Duration { get; set; }
}
