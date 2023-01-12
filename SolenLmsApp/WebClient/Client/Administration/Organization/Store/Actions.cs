using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Administration.Organization.Store;


public sealed record LoadOrganizationAction(CancellationToken CancellationToken);

public sealed record LoadOrganizationResultAction(GetOrganizationQueryResult Organization);
