using MediatR;
using OrdersBackend.Application.Orders.Commands.CreateOrder;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersBackend.Application.Orders.Commands.CancelOrder
{
    public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Guid>;

}
