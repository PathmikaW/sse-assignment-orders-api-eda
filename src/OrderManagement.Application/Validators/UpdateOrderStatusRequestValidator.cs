using FluentValidation;
using OrderManagement.Application.DTOs;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Validators;

/// <summary>
/// Validator for UpdateOrderStatusRequest.
/// </summary>
public sealed class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid order status.")
            .Must(status => status != OrderStatus.Pending)
            .WithMessage("Cannot set status back to Pending.");
    }
}
