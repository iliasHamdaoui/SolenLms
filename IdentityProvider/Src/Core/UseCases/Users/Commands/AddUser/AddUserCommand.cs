using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.AddUser;
public sealed record AddUserCommand : IRequest<RequestResponse>
{
    public string Email { get; set; } = default!;
    public string? FamilyName { get; set; }
    public string? GivenName { get; set; }
    public List<string> Roles { get; set; } = new();
}
