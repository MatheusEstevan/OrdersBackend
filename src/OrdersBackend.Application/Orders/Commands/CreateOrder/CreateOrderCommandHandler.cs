using MediatR;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdersBackend.Application.Orders.Commands.CreateOrder
{
    public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly TimeProvider _timeProvider;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, TimeProvider timeProvider)
        {
            _orderRepository = orderRepository;
            _timeProvider = timeProvider;
        }
        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Order newOrder = Order.Create(request.CustomerId, _timeProvider.GetUtcNow().UtcDateTime, request.Items.Select(i => (i.ProductName, i.Quantity, i.UnitPrice)));
            await _orderRepository.AddAsync(newOrder, cancellationToken);
            await _orderRepository.SaveChangesAsync(cancellationToken);
            return newOrder.Id;

        }
    }
}
