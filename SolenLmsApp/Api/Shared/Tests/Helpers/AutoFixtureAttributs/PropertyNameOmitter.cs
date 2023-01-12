using AutoFixture.Kernel;
using System.Reflection;

namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.AutoFixtureAttributs;


internal class PropertyNameOmitter : ISpecimenBuilder
{
    private readonly IEnumerable<string> _names;

    internal PropertyNameOmitter(params string[] names)
    {
        _names = names;
    }

    public object Create(object request, ISpecimenContext context)
    {
        var propInfo = request as PropertyInfo;
        if (propInfo != null && _names.Contains(propInfo.Name))
            return new OmitSpecimen();

        return new NoSpecimen();
    }
}


