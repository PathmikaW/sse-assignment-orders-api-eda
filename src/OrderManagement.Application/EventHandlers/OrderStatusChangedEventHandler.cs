using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Events;

namespace OrderManagement.Application.EventHandlers;

/// <summary>
/// Handles the OrderStatusChangedEvent domain event.
/// </summary>
public sealed class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
{
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;

    public OrderStatusChangedEventHandler(ILogger<OrderStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Event] OrderStatusChanged - OrderId: {OrderId}, OrderNumber: {OrderNumber}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
            notification.OrderId,
            notification.OrderNumber,
            notification.OldStatus,
            notification.NewStatus);

        // In a real microservices scenario, this handler could:
        // - Notify payment service when status is Paid
        // - Trigger refund process when status is Cancelled
        // - Update order tracking system

        return Task.CompletedTask;
    }
}
