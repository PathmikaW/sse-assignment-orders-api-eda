namespace OrderManagement.Application.DTOs;

/// <summary>
/// Request DTO for creating a new order.
/// </summary>
public sealed record CreateOrderRequest
{
    /// <summary>
    /// The customer's email address.
    /// </summary>
    public string CustomerEmail { get; init; } = string.Empty;

    /// <summary>
    /// The total amount of the order. Must be greater than zero.
    /// </summary>
    public decimal TotalAmount { get; init; }
}
