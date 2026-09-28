using MediatR;
using OrdersBackend.Application.Orders.Dtos;

namespace OrdersBackend.Application.Orders.Queries.GetAllOrders
{
    public sealed record GetAllOrdersQuery(int Page, int PageSize) : IRequest<GetAllOrdersResponse>;

    public sealed record GetAllOrdersResponse(IReadOnlyList<OrderDto> Items, int TotalPages);
}
