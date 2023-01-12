using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.DeleteUser;
public sealed record DeleteUserCommand : IRequest<RequestResponse>
{
    public string UserEmail { get; }

    public DeleteUserCommand(string userEmail)
    {
        UserEmail = userEmail;
    }
}
