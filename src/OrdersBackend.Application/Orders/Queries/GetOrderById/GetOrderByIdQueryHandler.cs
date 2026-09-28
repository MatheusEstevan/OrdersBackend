using MediatR;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Common.Exceptions;
using OrdersBackend.Application.Orders.Dtos;

namespace OrdersBackend.Application.Orders.Queries.GetOrderById
{
    public sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
                ?? throw new NotFoundException($"Pedido {request.OrderId} não encontrado.");

            return OrderDto.FromOrder(order);
        }
    }
}
