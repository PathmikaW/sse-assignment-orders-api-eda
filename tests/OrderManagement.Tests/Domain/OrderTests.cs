using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Tests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var email = "test@example.com";
        var amount = 100.50m;

        // Act
        var order = Order.Create(email, amount);

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().NotBeEmpty();
        order.OrderNumber.Should().StartWith("ORD-");
        order.CustomerEmail.Should().Be(email);
        order.TotalAmount.Should().Be(amount);
        order.Status.Should().Be(OrderStatus.Pending);
        order.OrderDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Create_WithInvalidAmount_ShouldThrowArgumentException(decimal amount)
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var act = () => Order.Create(email, amount);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Total amount must be greater than zero*");
    }

    [Theory]
    [InlineData(OrderStatus.Pending, OrderStatus.Paid, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Cancelled, true)]
    [InlineData(OrderStatus.Paid, OrderStatus.Cancelled, true)]
    [InlineData(OrderStatus.Paid, OrderStatus.Pending, false)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Pending, false)]
    [InlineData(OrderStatus.Cancelled, OrderStatus.Paid, false)]
    public void CanTransitionTo_ShouldReturnExpectedResult(
        OrderStatus currentStatus, OrderStatus newStatus, bool expected)
    {
        // Arrange
        var order = Order.Create("test@example.com", 100m);

        // Set initial status if not Pending
        if (currentStatus == OrderStatus.Paid)
        {
            order.UpdateStatus(OrderStatus.Paid);
        }
        else if (currentStatus == OrderStatus.Cancelled)
        {
            order.UpdateStatus(OrderStatus.Cancelled);
        }

        // Act
        var result = order.CanTransitionTo(newStatus);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void UpdateStatus_WithValidTransition_ShouldUpdateStatus()
    {
        // Arrange
        var order = Order.Create("test@example.com", 100m);

        // Act
        order.UpdateStatus(OrderStatus.Paid);

        // Assert
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void UpdateStatus_WithInvalidTransition_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var order = Order.Create("test@example.com", 100m);
        order.UpdateStatus(OrderStatus.Cancelled);

        // Act
        var act = () => order.UpdateStatus(OrderStatus.Paid);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition from Cancelled to Paid*");
    }
}
