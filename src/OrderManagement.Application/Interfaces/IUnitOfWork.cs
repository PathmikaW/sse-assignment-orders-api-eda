namespace OrderManagement.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
