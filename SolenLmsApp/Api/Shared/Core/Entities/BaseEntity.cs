namespace Imanys.SolenLms.Application.Shared.Core.Entities;

#nullable disable 


public abstract class BaseEntity<TId>
{
    public TId Id { get; init; }
}

public abstract class BaseEntity : BaseEntity<int>
{
}

public abstract class BaseAuditEntity<TId> : BaseEntity<TId>
{

    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime LastModifiedAt { get; set; }
    public int Version { get; set; }
}

public abstract class BaseAuditEntity : BaseEntity
{
    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string LastModifiedBy { get; set; }

    public DateTime LastModifiedAt { get; set; }
    public int Version { get; set; }
}

