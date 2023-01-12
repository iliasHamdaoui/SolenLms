namespace Imanys.SolenLms.Application.Shared.Tests.Helpers.Users;

public sealed class LearnerUser : TestUser
{
    public override string Role { get; init; } = "Learner";
}
