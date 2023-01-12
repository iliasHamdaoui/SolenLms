namespace Imanys.SolenLms.IdentityProvider.Core.Domain.Entities;

public sealed class Organization
{
    private readonly List<User> _users = new();
    public Organization(string id, string name, DateTime creationDate)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        Id = id;
        Name = name;
        CreationDate = creationDate;
    }

    public string Id { get; init; }

    public string Name { get; private set; }

    public DateTime CreationDate { get; init; }

    public IEnumerable<User> Users => _users.AsReadOnly();
    
    public void UpdateName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        Name = name;
    }
}
