using Microsoft.EntityFrameworkCore;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersBackend.Infrastructure.Persistence.Repositories
{
    internal sealed class OrderRepository(OrdersDbContext context) : IOrderRepository
    {
        public async Task AddAsync(Order order, CancellationToken cancellationToken)
        {
             context.Add(order);
        }

        public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            return await context.Orders.Include(i => i.Items).FirstOrDefaultAsync(f => f.Id == orderId, cancellationToken);
        }

        public async Task<IReadOnlyList<Order>> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await context.Orders.Include(i => i.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken)
        {
            return await context.Orders.CountAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
