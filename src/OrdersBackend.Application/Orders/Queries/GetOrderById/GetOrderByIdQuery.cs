using MediatR;
using OrdersBackend.Application.Orders.Dtos;

namespace OrdersBackend.Application.Orders.Queries.GetOrderById
{
    public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
}
