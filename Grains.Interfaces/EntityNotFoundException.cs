using System;

namespace Grains.Interfaces;

[Serializable]
public class EntityNotFoundException : EntityException
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public EntityNotFoundException(Type entityType) : base(entityType)
    {
    }

    public EntityNotFoundException(Type entityType, object entityId) : base(entityType, entityId)
    {
    }

    public EntityNotFoundException(Type entityType, string message) : base(entityType, message)
    {
    }

    public EntityNotFoundException(Type entityType, object entityId, string message)
        : base(entityType, entityId, message)
    {
    }

    public EntityNotFoundException(Type entityType, string message, Exception innerException)
        : base(entityType, message, innerException)
    {
    }

    public EntityNotFoundException(Type entityType, object entityId, string message, Exception innerException)
        : base(entityType, entityId, message, innerException)
    {
    }
}