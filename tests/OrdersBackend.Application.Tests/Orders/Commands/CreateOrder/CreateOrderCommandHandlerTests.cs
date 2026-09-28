using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Orders.Commands.CreateOrder;
using OrdersBackend.Domain.Common;
using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Application.Tests.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandlerTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 15, 10, 30, 0, TimeSpan.Zero);

    private readonly IOrderRepository _repository = Substitute.For<IOrderRepository>();
    private readonly FakeTimeProvider _timeProvider = new(Now);
    private readonly CreateOrderCommandHandler _handler;

    private Order? _addedOrder;

    public CreateOrderCommandHandlerTests()
    {
        _repository
            .AddAsync(Arg.Do<Order>(order => _addedOrder = order), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _handler = new CreateOrderCommandHandler(_repository, _timeProvider);
    }

    private static CreateOrderCommand ValidCommand() => new(
        Guid.NewGuid(),
        [new CreateOrderItem("Keyboard", 2, 150.00m), new CreateOrderItem("Mouse", 1, 80.00m)]);

    [Fact]
    public async Task Handle_WithValidCommand_AddsOrderWithCommandData()
    {
        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(_addedOrder);
        Assert.Equal(command.CustomerId, _addedOrder.CustomerId);
        Assert.Equal(OrderStatus.Pending, _addedOrder.Status);
        Assert.Equal(2, _addedOrder.Items.Count);

        var keyboard = Assert.Single(_addedOrder.Items, i => i.ProductName == "Keyboard");
        Assert.Equal(2, keyboard.Quantity);
        Assert.Equal(150.00m, keyboard.UnitPrice);
    }

    [Fact]
    public async Task Handle_WithValidCommand_SetsCreatedAtFromTimeProvider()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.NotNull(_addedOrder);
        Assert.Equal(Now.UtcDateTime, _addedOrder.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, _addedOrder.CreatedAt.Kind);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsIdOfAddedOrder()
    {
        var orderId = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.NotNull(_addedOrder);
        Assert.NotEqual(Guid.Empty, orderId);
        Assert.Equal(_addedOrder.Id, orderId);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsThenSavesExactlyOnce()
    {
        await _handler.Handle(ValidCommand(), CancellationToken.None);

        Received.InOrder(() =>
        {
            _repository.AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
            _repository.SaveChangesAsync(Arg.Any<CancellationToken>());
        });
        await _repository.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToRepository()
    {
        using var cancellationSource = new CancellationTokenSource();
        var token = cancellationSource.Token;

        await _handler.Handle(ValidCommand(), token);

        await _repository.Received(1).AddAsync(Arg.Any<Order>(), token);
        await _repository.Received(1).SaveChangesAsync(token);
    }

    [Fact]
    public async Task Handle_WhenDomainRejectsItem_ThrowsDomainExceptionAndDoesNotPersist()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), [new CreateOrderItem("Keyboard", 0, 150.00m)]);

        await Assert.ThrowsAsync<DomainException>(() => _handler.Handle(command, CancellationToken.None));

        await _repository.DidNotReceive().AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
