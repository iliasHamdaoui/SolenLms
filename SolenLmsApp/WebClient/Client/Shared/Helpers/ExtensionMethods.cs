namespace Imanys.SolenLms.Application.WebClient.Shared.Helpers;

public static class ExtensionMethods
{
    public static string ToHours(this int durationInSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(durationInSeconds);
        return time.ToString(@"hh\:mm\:ss");
    }
}
