using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Interfaces;
using OrderManagement.Application.Services;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Domain.Events;

namespace OrderManagement.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<OrderService>>();

        _sut = new OrderService(
            _orderRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidRequest_ShouldCreateOrderAndPublishEvent()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = "test@example.com",
            TotalAmount = 150.00m
        };

        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.CreateOrderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CustomerEmail.Should().Be(request.CustomerEmail);
        result.TotalAmount.Should().Be(request.TotalAmount);
        result.Status.Should().Be(OrderStatus.Pending);
        result.OrderNumber.Should().StartWith("ORD-");

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithExistingOrder_ShouldReturnOrder()
    {
        // Arrange
        var order = Order.Create("test@example.com", 100m);
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _sut.GetOrderByIdAsync(order.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.CustomerEmail.Should().Be(order.CustomerEmail);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WithNonExistingOrder_ShouldReturnNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.GetOrderByIdAsync(orderId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithValidTransition_ShouldUpdateAndPublishEvent()
    {
        // Arrange
        var order = Order.Create("test@example.com", 100m);
        var request = new UpdateOrderStatusRequest { NewStatus = OrderStatus.Paid };

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(order.Id, request);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(OrderStatus.Paid);

        _orderRepositoryMock.Verify(x => x.Update(order), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithNonExistingOrder_ShouldReturnNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var request = new UpdateOrderStatusRequest { NewStatus = OrderStatus.Paid };

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.UpdateOrderStatusAsync(orderId, request);

        // Assert
        result.Should().BeNull();
        _mediatorMock.Verify(x => x.Publish(It.IsAny<OrderStatusChangedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteOrderAsync_WithExistingOrder_ShouldReturnTrue()
    {
        // Arrange
        var order = Order.Create("test@example.com", 100m);

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.DeleteOrderAsync(order.Id);

        // Assert
        result.Should().BeTrue();
        _orderRepositoryMock.Verify(x => x.Delete(order), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteOrderAsync_WithNonExistingOrder_ShouldReturnFalse()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _orderRepositoryMock.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _sut.DeleteOrderAsync(orderId);

        // Assert
        result.Should().BeFalse();
        _orderRepositoryMock.Verify(x => x.Delete(It.IsAny<Order>()), Times.Never);
    }
}
