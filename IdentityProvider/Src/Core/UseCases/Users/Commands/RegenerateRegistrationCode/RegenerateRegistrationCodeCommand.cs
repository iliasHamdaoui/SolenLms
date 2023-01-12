using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Commands.RegenerateRegistrationCode;
public sealed record RegenerateRegistrationCodeCommand : IRequest<RequestResponse>
{
    public string Email { get; set; } = default!;
}
