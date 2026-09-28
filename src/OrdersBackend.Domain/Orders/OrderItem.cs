using OrdersBackend.Domain.Common;

namespace OrdersBackend.Domain.Orders;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem()
    {
    }

    internal static OrderItem Create(Guid orderId, string productName, int quantity, decimal unitPrice)
    {
        if(quantity <= 0) {
            throw new DomainException("Quantidade deve ser maior que 0");
        }
        if(unitPrice <= 0) {
            throw new DomainException("Preço unitário deve ser maior que 0");
        }
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
