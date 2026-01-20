namespace OrderManagement.Domain.Enums;

/// <summary>
/// Represents the possible statuses of an order.
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Order has been created but not yet paid.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Order has been paid successfully.
    /// </summary>
    Paid = 1,

    /// <summary>
    /// Order has been cancelled.
    /// </summary>
    Cancelled = 2
}
