namespace Imanys.SolenLms.Application.Shared.Core.Enums;

public sealed record class MediaType : Enumeration
{
    public static readonly MediaType Text = new("text/html", nameof(Text));
    public static readonly MediaType Video = new("video/mp4", nameof(Video));
    private MediaType(string value, string name) : base(value, name)
    {

    }
}
