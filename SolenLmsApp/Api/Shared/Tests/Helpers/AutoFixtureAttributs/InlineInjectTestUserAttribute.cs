using AutoFixture;
using AutoFixture.Xunit2;

namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.AutoFixtureAttributs;



[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InlineInjectTestUserAttribute : InlineAutoDataAttribute
{
    public InlineInjectTestUserAttribute(params object[] values)
         : base(new InjectTestUserAttribute(), values)
    {
    }

    private class InstructorCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new PropertyNameOmitter("Role"));
        }
    }
}
