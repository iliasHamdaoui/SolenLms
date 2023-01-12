using Imanys.SolenLms.Application.WebClient.Administration.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Administration.Users.Store;


public sealed record LoadUsersAction(CancellationToken CancellationToken);

public sealed record LoadUsersResultAction(GetUsersQueryResult QueryResult);
