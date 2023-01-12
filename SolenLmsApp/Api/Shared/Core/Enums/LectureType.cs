namespace Imanys.SolenLms.Application.Shared.Core.Enums;

public sealed record LectureType : Enumeration
{
    public static readonly LectureType Article = new("Article", nameof(Article), MediaType.Text);
    public static readonly LectureType Video = new("Video", nameof(Video), MediaType.Video);
    private LectureType(string value, string name, MediaType? mediaType = null) : base(value, name)
    {
        MediaType = mediaType;
    }

    public MediaType? MediaType { get; }
}
