using System.ComponentModel.DataAnnotations;

namespace Imanys.SolenLms.Application.WebClient.Shared.Helpers;

public static class Utils
{
    public static int MaxLength<T>(string property)
    {
        var attribute = typeof(T).GetProperties()
                                  .Where(p => p.Name == property)
                                  .Single()
                                  .GetCustomAttributes(typeof(StringLengthAttribute), true)
                                  .Single() as StringLengthAttribute;

        return attribute?.MaximumLength ?? 0;
    }
}
