using MediatR;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Orders.Dtos;

namespace OrdersBackend.Application.Orders.Queries.GetAllOrders
{
    public sealed class GetAllOrdersQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetAllOrdersQuery, GetAllOrdersResponse>
    {
        public async Task<GetAllOrdersResponse> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await orderRepository.ListAsync(request.Page, request.PageSize, cancellationToken);
            var totalCount = await orderRepository.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new GetAllOrdersResponse(orders.Select(s => OrderDto.FromOrder(s)).ToList(), totalPages);
        }
    }
}
