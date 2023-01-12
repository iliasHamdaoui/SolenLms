using System.Reflection;

#nullable disable

namespace Imanys.SolenLms.Application.Shared.Core.Enums;

public abstract record Enumeration : IComparable
{
    public string Value { get; }

    public string Name { get; }

    protected Enumeration()
    {
    }

    protected Enumeration(string value, string name)
    {
        Value = value;
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T).GetFields(BindingFlags.Public |
                      BindingFlags.Static |
                      BindingFlags.DeclaredOnly)
           .Select(f => f.GetValue(null))
           .Cast<T>();
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }


    public static (bool success, T item) FromValue<T>(string value) where T : Enumeration
    {
        var (success, matchingItem) = Parse<T>(item => string.Equals(item.Value, value, StringComparison.CurrentCultureIgnoreCase));
        return (success, matchingItem);
    }
    
    public static bool TryConvertFromValue<T>(string value, out T type) where T : Enumeration
    {
        (bool success, type) = Parse<T>(item => string.Equals(item.Value, value, StringComparison.CurrentCultureIgnoreCase));
        return success;
    }

    public static (bool success, T item) FromName<T>(string name) where T : Enumeration
    {
        var (success, matchingItem) = Parse<T>(item =>
            string.Equals(item.Name, name, StringComparison.CurrentCultureIgnoreCase));
        return (success, matchingItem);
    }

    private static (bool success, T item) Parse<T>(Func<T, bool> predicate)
        where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem != null)
            return (true, matchingItem);

        return (false, default);
    }

    public int CompareTo(object other)
    {
        return Value.CompareTo(((Enumeration)other).Value);
    }
}
