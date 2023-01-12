using static Imanys.SolenLms.Application.Shared.Core.Enums.UserRole;

namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

public sealed class InstructorUser : TestUser
{
    public override string Role { get; init; } = Instructor;
}
