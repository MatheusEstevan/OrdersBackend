using OrdersBackend.Domain.Common;
using System.Collections.Generic;

namespace OrdersBackend.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public decimal TotalAmount => _items.Sum(s => s.Quantity * s.UnitPrice);

    private Order()
    {
    }

    public static Order Create(Guid customerId, DateTime createdAt,
                                 IEnumerable<(string ProductName, int Quantity, decimal UnitPrice)> items)
    {
        var itemsList = items.ToList();
        if (customerId == Guid.Empty)
        {
            throw new DomainException("CustomerId não pode ser vazio");
        }
        if(itemsList.Count == 0) {
            throw new DomainException("Um pedido deve ter pelo menos um item");
        }

        Order order = new Order     
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            CreatedAt = createdAt
        };
        order._items.AddRange(itemsList.Select(i => OrderItem.Create(order.Id, i.ProductName, i.Quantity, i.UnitPrice)));
        return order;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainException("Somente pedidos pendentes podem ser cancelados");
        }
        Status = OrderStatus.Cancelled;
    }
}
