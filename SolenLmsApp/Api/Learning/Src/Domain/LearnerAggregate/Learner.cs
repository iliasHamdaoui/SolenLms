using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.LearnerAggregate;

public sealed class Learner : IAggregateRoot
{
    public Learner(string id, string organizationId)
    {
        Id = id;
        OrganizationId = organizationId;
    }

    public string Id { get; init; }
    public string OrganizationId { get; init; }
}