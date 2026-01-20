using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Entities;

/// <summary>
/// Represents an order in the system.
/// </summary>
public class Order
{
    public Guid Id { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public DateTime OrderDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Required for EF Core materialization.
    /// </summary>
    private Order() { }

    /// <summary>
    /// Creates a new order with the specified customer email and total amount.
    /// </summary>
    /// <param name="customerEmail">The customer's email address.</param>
    /// <param name="totalAmount">The total amount of the order (must be greater than zero).</param>
    /// <returns>A new Order instance.</returns>
    /// <exception cref="ArgumentException">Thrown when totalAmount is less than or equal to zero.</exception>
    public static Order Create(string customerEmail, decimal totalAmount)
    {
        if (totalAmount <= 0)
            throw new ArgumentException("Total amount must be greater than zero.", nameof(totalAmount));

        return new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            CustomerEmail = customerEmail,
            OrderDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending
        };
    }

    /// <summary>
    /// Determines whether a transition to the specified status is valid.
    /// </summary>
    /// <param name="newStatus">The target status.</param>
    /// <returns>True if the transition is valid; otherwise, false.</returns>
    public bool CanTransitionTo(OrderStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Paid) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Paid, OrderStatus.Cancelled) => true,
            _ => false
        };
    }

    /// <summary>
    /// Updates the order status to the specified value.
    /// </summary>
    /// <param name="newStatus">The new status.</param>
    /// <exception cref="InvalidOperationException">Thrown when the status transition is not allowed.</exception>
    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException(
                $"Cannot transition from {Status} to {newStatus}.");

        Status = newStatus;
    }

    /// <summary>
    /// Generates a unique order number.
    /// Format: ORD-{yyyyMMdd}-{random8chars}
    /// </summary>
    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";
    }
}
