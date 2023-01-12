using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganization;
internal sealed class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, RequestResponse<GetOrganizationQueryResult>>
{
    private readonly IOrganizationService _service;

    public GetOrganizationQueryHandler(IOrganizationService service)
    {
        _service = service;
    }

    public async Task<RequestResponse<GetOrganizationQueryResult>> Handle(GetOrganizationQuery query, CancellationToken cancellationToken)
    {
        var organization = await _service.GetTheCurrentUserOrganization(cancellationToken);
        if (organization == null)
            return RequestResponse<GetOrganizationQueryResult>.Error(ResponseError.NotFound, "The organization does not exist.");

        var result = new GetOrganizationQueryResult
        {
            OrganizationName = organization.Name,
            CreationDate = organization.CreationDate,
            UserCount = organization.Users.Count(),
        };

        return RequestResponse<GetOrganizationQueryResult>.Ok(data: result);
    }
}
