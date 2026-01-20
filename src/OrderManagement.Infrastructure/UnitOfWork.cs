using OrderManagement.Application.Interfaces;
using OrderManagement.Infrastructure.Data;

namespace OrderManagement.Infrastructure;

/// <summary>
/// Unit of Work implementation for managing transactions.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OrderDbContext _context;

    public UnitOfWork(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
