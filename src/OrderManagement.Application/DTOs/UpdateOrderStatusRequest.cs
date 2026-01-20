using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

/// <summary>
/// Request DTO for updating an order's status.
/// </summary>
public sealed record UpdateOrderStatusRequest
{
    /// <summary>
    /// The new status for the order.
    /// </summary>
    public OrderStatus NewStatus { get; init; }
}
