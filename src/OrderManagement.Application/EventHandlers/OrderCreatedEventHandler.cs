using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Domain.Events;

namespace OrderManagement.Application.EventHandlers;

/// <summary>
/// Handles the OrderCreatedEvent domain event.
/// </summary>
public sealed class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "[Event] OrderCreated - OrderId: {OrderId}, OrderNumber: {OrderNumber}, CustomerEmail: {CustomerEmail}, TotalAmount: {TotalAmount:C}",
            notification.OrderId,
            notification.OrderNumber,
            notification.CustomerEmail,
            notification.TotalAmount);

        // In a real microservices scenario, this handler could:
        // - Send a confirmation email
        // - Notify inventory service
        // - Update analytics/reporting

        return Task.CompletedTask;
    }
}
