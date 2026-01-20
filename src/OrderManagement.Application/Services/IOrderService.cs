using OrderManagement.Application.DTOs;

namespace OrderManagement.Application.Services;

/// <summary>
/// Service interface for order operations.
/// </summary>
public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse?> GetOrderByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderResponse>> SearchOrdersAsync(OrderSearchParameters parameters, CancellationToken cancellationToken = default);
    Task<OrderResponse?> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteOrderAsync(Guid id, CancellationToken cancellationToken = default);
}
