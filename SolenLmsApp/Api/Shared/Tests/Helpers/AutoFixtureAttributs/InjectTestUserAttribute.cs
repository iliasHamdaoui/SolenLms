using AutoFixture;
using AutoFixture.Xunit2;

namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.AutoFixtureAttributs;



[AttributeUsage(AttributeTargets.Method)]
public sealed class InjectTestUserAttribute : AutoDataAttribute
{
    public InjectTestUserAttribute()
         : base(() => new Fixture().Customize(new TestUserWithoutRoleCustomization()))
    {
    }

    private class TestUserWithoutRoleCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new PropertyNameOmitter("Role"));
        }
    }
}


//[AttributeUsage(AttributeTargets.Method)]
//public sealed class InjectTestUserAttribute : AutoDataAttribute
//{
//    public InjectTestUserAttribute()
//         : base(() => new Fixture().Customize(new InstructorCustomization()))
//    {
//    }

//    private class InstructorCustomization : ICustomization
//    {
//        public void Customize(IFixture fixture)
//        {
//            var instructor = fixture.Build<TestUser>().With(x => x.Role, "Instructor");

//            fixture.Register(() => instructor.Create());
//        }
//    }
//}