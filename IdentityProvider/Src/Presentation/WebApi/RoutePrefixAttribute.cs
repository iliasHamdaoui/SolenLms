namespace Imanys.SolenLms.IdentityProvider.WebApi;



[AttributeUsage(AttributeTargets.Class)]
public sealed class RoutePrefixAttribute : Attribute
{
    public RoutePrefixAttribute(string prefix) { Prefix = prefix; }
    public string Prefix { get; }
}