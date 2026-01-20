using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.DTOs;

/// <summary>
/// Parameters for searching orders.
/// </summary>
public sealed record OrderSearchParameters
{
    /// <summary>
    /// Filter by customer email (partial match).
    /// </summary>
    public string? CustomerEmail { get; init; }

    /// <summary>
    /// Filter by order status.
    /// </summary>
    public OrderStatus? Status { get; init; }

    /// <summary>
    /// Filter orders from this date (inclusive, UTC).
    /// </summary>
    public DateTime? FromDate { get; init; }

    /// <summary>
    /// Filter orders until this date (inclusive, UTC).
    /// </summary>
    public DateTime? ToDate { get; init; }
}
