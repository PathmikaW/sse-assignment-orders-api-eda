using FluentAssertions;
using FluentValidation.TestHelper;
using OrderManagement.Application.DTOs;
using OrderManagement.Application.Validators;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Tests.Application;

public class ValidatorTests
{
    private readonly CreateOrderRequestValidator _createOrderValidator;
    private readonly UpdateOrderStatusRequestValidator _updateStatusValidator;

    public ValidatorTests()
    {
        _createOrderValidator = new CreateOrderRequestValidator();
        _updateStatusValidator = new UpdateOrderStatusRequestValidator();
    }

    #region CreateOrderRequestValidator Tests

    [Fact]
    public void CreateOrderValidator_WithValidRequest_ShouldNotHaveErrors()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = "test@example.com",
            TotalAmount = 100.50m
        };

        // Act
        var result = _createOrderValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void CreateOrderValidator_WithEmptyEmail_ShouldHaveError(string? email)
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = email!,
            TotalAmount = 100m
        };

        // Act
        var result = _createOrderValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("invalid.com")]
    public void CreateOrderValidator_WithInvalidEmail_ShouldHaveError(string email)
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = email,
            TotalAmount = 100m
        };

        // Act
        var result = _createOrderValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CustomerEmail)
            .WithErrorMessage("A valid email address is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void CreateOrderValidator_WithInvalidAmount_ShouldHaveError(decimal amount)
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = "test@example.com",
            TotalAmount = amount
        };

        // Act
        var result = _createOrderValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount)
            .WithErrorMessage("Total amount must be greater than zero.");
    }

    #endregion

    #region UpdateOrderStatusRequestValidator Tests

    [Theory]
    [InlineData(OrderStatus.Paid)]
    [InlineData(OrderStatus.Cancelled)]
    public void UpdateStatusValidator_WithValidStatus_ShouldNotHaveErrors(OrderStatus status)
    {
        // Arrange
        var request = new UpdateOrderStatusRequest { NewStatus = status };

        // Act
        var result = _updateStatusValidator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateStatusValidator_WithPendingStatus_ShouldHaveError()
    {
        // Arrange
        var request = new UpdateOrderStatusRequest { NewStatus = OrderStatus.Pending };

        // Act
        var result = _updateStatusValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewStatus)
            .WithErrorMessage("Cannot set status back to Pending.");
    }

    [Fact]
    public void UpdateStatusValidator_WithInvalidEnumValue_ShouldHaveError()
    {
        // Arrange
        var request = new UpdateOrderStatusRequest { NewStatus = (OrderStatus)999 };

        // Act
        var result = _updateStatusValidator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewStatus);
    }

    #endregion
}
