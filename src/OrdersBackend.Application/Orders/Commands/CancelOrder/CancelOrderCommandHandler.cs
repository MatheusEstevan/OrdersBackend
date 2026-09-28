using MediatR;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersBackend.Application.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<CancelOrderCommand, Guid>
    {

        public async Task<Guid> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
                ?? throw new NotFoundException($"Pedido {request.OrderId} não encontrado.");

            order.Cancel();
            await orderRepository.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
