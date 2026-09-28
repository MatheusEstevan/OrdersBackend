using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Application.Orders.Dtos
{
    public sealed record OrderDto(
        Guid Id,
        Guid CustomerId,
        string Status,
        DateTime CreatedAt,
        decimal TotalAmount,
        IReadOnlyList<OrderItemDto> Items)
    {
        public static OrderDto FromOrder(Order order)
        {
            return new OrderDto(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.CreatedAt,
                order.TotalAmount,
                order.Items.Select(s => new OrderItemDto(s.Id, s.ProductName, s.Quantity, s.UnitPrice)).ToList());
        }
    }

    public sealed record OrderItemDto(Guid Id, string ProductName, int Quantity, decimal UnitPrice);
}
