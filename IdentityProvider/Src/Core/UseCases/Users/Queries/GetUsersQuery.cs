using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Queries;
public sealed record GetUsersQuery : IRequest<RequestResponse<GetUsersQueryResult>>
{
}
