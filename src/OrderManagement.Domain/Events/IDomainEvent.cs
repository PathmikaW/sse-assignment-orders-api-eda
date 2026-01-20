using MediatR;

namespace OrderManagement.Domain.Events;

/// <summary>
/// Marker interface for domain events.
/// Implements INotification to integrate with MediatR for event publishing.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// The unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// The UTC timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOnUtc { get; }
}
