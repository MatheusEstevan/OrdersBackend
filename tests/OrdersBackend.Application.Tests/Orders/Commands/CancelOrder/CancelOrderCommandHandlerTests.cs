using NSubstitute;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Common.Exceptions;
using OrdersBackend.Application.Orders.Commands.CancelOrder;
using OrdersBackend.Domain.Common;
using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Application.Tests.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandlerTests
{
    private readonly IOrderRepository _repository = Substitute.For<IOrderRepository>();
    private readonly CancelOrderCommandHandler _handler;

    public CancelOrderCommandHandlerTests()
    {
        _handler = new CancelOrderCommandHandler(_repository);
    }

    private static Order PendingOrder() =>
        Order.Create(Guid.NewGuid(), new DateTime(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc), [("Keyboard", 1, 150.00m)]);

    [Fact]
    public async Task Handle_WithPendingOrder_CancelsAndSaves()
    {
        var order = PendingOrder();
        _repository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, result);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToRepository()
    {
        var order = PendingOrder();
        using var cancellationSource = new CancellationTokenSource();
        var token = cancellationSource.Token;
        _repository.GetByIdAsync(order.Id, token).Returns(order);

        await _handler.Handle(new CancelOrderCommand(order.Id), token);

        await _repository.Received(1).GetByIdAsync(order.Id, token);
        await _repository.Received(1).SaveChangesAsync(token);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ThrowsNotFoundExceptionAndDoesNotSave()
    {
        var orderId = Guid.NewGuid();
        _repository.GetByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new CancelOrderCommand(orderId), CancellationToken.None));

        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenOrderIsAlreadyCancelled_ThrowsDomainExceptionAndDoesNotSave()
    {
        var order = PendingOrder();
        order.Cancel();
        _repository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        await Assert.ThrowsAsync<DomainException>(
            () => _handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None));

        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
