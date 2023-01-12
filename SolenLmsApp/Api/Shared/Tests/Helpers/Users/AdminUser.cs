using static Imanys.SolenLms.Application.Shared.Core.Enums.UserRole;

namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

public sealed class AdminUser : TestUser
{
    public override string Role { get; init; } = Admin;
}
