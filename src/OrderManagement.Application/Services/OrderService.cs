using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Events;

namespace OrderManagement.Application.Services;

/// <summary>
/// Service implementation for order operations.
/// </summary>
public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = Order.Create(request.CustomerEmail, request.TotalAmount);

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderNumber} created for {CustomerEmail}", order.OrderNumber, order.CustomerEmail);

        // Publish domain event
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount
        };
        await _mediator.Publish(orderCreatedEvent, cancellationToken);

        return MapToResponse(order);
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<OrderResponse>> SearchOrdersAsync(OrderSearchParameters parameters, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.SearchAsync(
            parameters.CustomerEmail,
            parameters.Status,
            parameters.FromDate,
            parameters.ToDate,
            cancellationToken);

        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponse?> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return null;

        var oldStatus = order.Status;
        order.UpdateStatus(request.NewStatus);

        _orderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderNumber} status changed from {OldStatus} to {NewStatus}",
            order.OrderNumber, oldStatus, request.NewStatus);

        // Publish domain event
        var statusChangedEvent = new OrderStatusChangedEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            OldStatus = oldStatus,
            NewStatus = request.NewStatus
        };
        await _mediator.Publish(statusChangedEvent, cancellationToken);

        return MapToResponse(order);
    }

    public async Task<bool> DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
            return false;

        _orderRepository.Delete(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order {OrderNumber} deleted", order.OrderNumber);

        return true;
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerEmail = order.CustomerEmail,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status
        };
    }
}
