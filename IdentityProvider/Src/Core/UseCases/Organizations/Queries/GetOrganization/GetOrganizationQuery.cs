using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganization;
public sealed record GetOrganizationQuery : IRequest<RequestResponse<GetOrganizationQueryResult>>
{
}
