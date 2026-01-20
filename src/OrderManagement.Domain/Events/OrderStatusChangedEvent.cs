using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Events;

/// <summary>
/// Domain event raised when an order's status changes.
/// </summary>
public sealed record OrderStatusChangedEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public Guid OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public OrderStatus OldStatus { get; init; }
    public OrderStatus NewStatus { get; init; }
}
