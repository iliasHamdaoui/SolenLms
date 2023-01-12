using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;
using System.Security.Claims;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Queries;
internal sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, RequestResponse<GetUsersQueryResult>>
{
    private readonly IOrganizationService _organizationService;
    private readonly ICurrentUser _currentUser;
    private readonly IAccountService _accountService;

    public GetUsersQueryHandler(IOrganizationService organizationService, ICurrentUser currentUser, IAccountService accountService)
    {
        _organizationService = organizationService;
        _currentUser = currentUser;
        _accountService = accountService;
    }

    public async Task<RequestResponse<GetUsersQueryResult>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _organizationService.GetOrganizationUsers(_currentUser.OrganizationId, cancellationToken);

        var usersIds = users.Select(x => x.Id).ToList();

        var usersClaims = await _accountService.GetUsersClaims(usersIds);

        var usersToReturn = new List<UserForGetUsersQueryResult>();
        foreach (var user in users)
        {
            usersToReturn.Add(new UserForGetUsersQueryResult
            {
                Id = user.Id,
                FamilyName = user.FamilyName,
                GivenName = user.GivenName,
                Email = user.Email,
                Roles = usersClaims.Where(x => x.UserId == user.Id && x.ClaimType == ClaimTypes.Role).Select(x => x.ClaimValue).ToList()!,
                Status = user.Active ? "Active" : "Pending"
            });
        }

        return RequestResponse<GetUsersQueryResult>.Ok(data: new GetUsersQueryResult(usersToReturn));
    }
}
