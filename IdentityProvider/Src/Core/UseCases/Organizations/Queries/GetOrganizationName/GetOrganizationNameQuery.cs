using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganizationName;

public sealed record GetOrganizationNameQuery : IRequest<RequestResponse<string>>
{
}
