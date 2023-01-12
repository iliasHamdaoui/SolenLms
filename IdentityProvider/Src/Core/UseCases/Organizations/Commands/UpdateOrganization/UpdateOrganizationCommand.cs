using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Commands.UpdateOrganization;
public sealed record UpdateOrganizationCommand : IRequest<RequestResponse>
{
    public string OrganizationName { get; set; } = default!;
}
