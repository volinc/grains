using System;

namespace Grains.Interfaces;

[Serializable]
public class EntityException : DomainException
{
    public EntityException()
    {
    }

    public EntityException(string message) : base(message)
    {
    }

    public EntityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public EntityException(Type entityType)
    {
        EntityType = entityType;
    }

    public EntityException(Type entityType, object entityId)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public EntityException(Type entityType, string message)
        : base(message)
    {
        EntityType = entityType;
    }

    public EntityException(Type entityType, object entityId, string message)
        : base(message)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public EntityException(Type entityType, string message, Exception innerException)
        : base(message, innerException)
    {
        EntityType = entityType;
    }

    public EntityException(Type entityType, object entityId, string message, Exception innerException)
        : base(message, innerException)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public Type EntityType { get; }

    public object EntityId { get; }
}