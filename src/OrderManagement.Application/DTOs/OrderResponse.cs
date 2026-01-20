using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

/// <summary>
/// Response DTO representing an order.
/// </summary>
public sealed record OrderResponse
{
    public Guid Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; }
    public string StatusDescription => Status.ToString();
}
