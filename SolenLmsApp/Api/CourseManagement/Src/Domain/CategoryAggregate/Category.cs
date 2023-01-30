using Imanys.SolenLms.Application.Shared.Core.Entities;

namespace Imanys.SolenLms.Application.CourseManagement.Core.Domain.CategoryAggregate;

public sealed class Category : BaseEntity, IAggregateRoot
{
    public Category(string organizationId, string name)
    {
        ArgumentNullException.ThrowIfNull(organizationId, nameof(organizationId));
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        OrganizationId = organizationId;
        Name = name;
    }

    public string OrganizationId { get; init; }
    public string Name { get; private set; }

    public void UpdateName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        Name = name;
    }
}
