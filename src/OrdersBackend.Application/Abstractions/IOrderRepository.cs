using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Application.Abstractions
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order, CancellationToken cancellationToken);
        Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<Order>> ListAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<int> CountAsync(CancellationToken cancellationToken);
    }
}
