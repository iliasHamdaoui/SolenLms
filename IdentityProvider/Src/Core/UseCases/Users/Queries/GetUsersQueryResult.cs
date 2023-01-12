namespace Imanys.SolenLms.IdentityProvider.Core.UseCases.Users.Queries;
public sealed record GetUsersQueryResult
{
    public IEnumerable<UserForGetUsersQueryResult> Users { get; }

    public GetUsersQueryResult(IEnumerable<UserForGetUsersQueryResult> users)
    {
        Users = users;
    }
}


public sealed record UserForGetUsersQueryResult
{
    public string Id { get; set; } = default!;
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }
    public string? Email { get; set; }
    public IEnumerable<string> Roles { get; set; } = default!;
    public string Status { get; set; } = default!;
}