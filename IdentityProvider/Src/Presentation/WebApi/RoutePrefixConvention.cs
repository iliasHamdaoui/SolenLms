using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Imanys.SolenLms.IdentityProvider.WebApi;

#nullable disable
public sealed class RoutePrefixConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        foreach (var selector in controller.Selectors)
        {
            var prefixes = GetPrefixes(controller.ControllerType);  // [prefix, parentPrefix, grandpaPrefix,...]
            if (prefixes.Count == 0) continue;
            // combine these prefixes one by one
            var prefixRouteModels = prefixes.Select(p => new AttributeRouteModel(new RouteAttribute(p.Prefix)))
                .Aggregate((acc, prefix) => AttributeRouteModel.CombineAttributeRouteModel(prefix, acc)!);
            selector.AttributeRouteModel = selector.AttributeRouteModel != null ?
                AttributeRouteModel.CombineAttributeRouteModel(prefixRouteModels, selector.AttributeRouteModel) :
                selector.AttributeRouteModel = prefixRouteModels;
        }
    }

    private IList<RoutePrefixAttribute> GetPrefixes(Type controlerType)
    {
        var list = new List<RoutePrefixAttribute>();
        FindPrefixesRec(controlerType, ref list);
        list = list.Where(r => r != null).ToList();
        return list;

        // find [MyRoutePrefixAttribute('...')] recursively 
        void FindPrefixesRec(Type type, ref List<RoutePrefixAttribute> results)
        {
            var prefix = type.GetCustomAttributes(false).OfType<RoutePrefixAttribute>().FirstOrDefault();
            results.Add(prefix);   // null is valid because it will seek prefix from parent recursively
            var parentType = type.BaseType;
            if (parentType == null) return;
            FindPrefixesRec(parentType, ref results);
        }
    }
}
