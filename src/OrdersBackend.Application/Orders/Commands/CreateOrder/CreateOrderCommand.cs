using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersBackend.Application.Orders.Commands.CreateOrder
{
    public sealed record CreateOrderCommand(Guid CustomerId, IReadOnlyList<CreateOrderItem> Items) : IRequest<Guid>;
    public sealed record CreateOrderItem(string ProductName, int Quantity, decimal UnitPrice);
}
