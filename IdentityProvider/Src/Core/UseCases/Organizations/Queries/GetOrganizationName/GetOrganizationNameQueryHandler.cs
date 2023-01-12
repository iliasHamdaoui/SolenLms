using Imanys.SolenLms.Application.Shared.Core;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using MediatR;

namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Organizations.Queries.GetOrganizationName;


internal sealed class GetOrganizationNameQueryHandler : IRequestHandler<GetOrganizationNameQuery, RequestResponse<string>>
{
    private readonly IOrganizationService _service;

    public GetOrganizationNameQueryHandler(IOrganizationService service)
    {
        _service = service;
    }

    public async Task<RequestResponse<string>> Handle(GetOrganizationNameQuery query, CancellationToken cancellationToken)
    {
        var organization = await _service.GetTheCurrentUserOrganization(cancellationToken);
        if (organization == null)
            return RequestResponse<string>.Error(ResponseError.NotFound, "The organization does not exist.");


        return RequestResponse<string>.Ok(data: organization.Name);
    }
}