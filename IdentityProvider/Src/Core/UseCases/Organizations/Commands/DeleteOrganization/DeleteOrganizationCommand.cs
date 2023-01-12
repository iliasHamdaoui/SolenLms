using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.DeleteOrganization;
public sealed record DeleteOrganizationCommand : IRequest<RequestResponse>
{
}
