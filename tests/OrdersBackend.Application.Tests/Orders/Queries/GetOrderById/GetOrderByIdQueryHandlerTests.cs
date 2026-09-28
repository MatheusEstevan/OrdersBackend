using NSubstitute;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Common.Exceptions;
using OrdersBackend.Application.Orders.Queries.GetOrderById;
using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Application.Tests.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandlerTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

    private readonly IOrderRepository _repository = Substitute.For<IOrderRepository>();
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _handler = new GetOrderByIdQueryHandler(_repository);
    }

    private static Order ExistingOrder() =>
        Order.Create(Guid.NewGuid(), CreatedAt, [("Keyboard", 2, 150.00m), ("Mouse", 1, 80.00m)]);

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsOrderData()
    {
        var order = ExistingOrder();
        _repository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        Assert.Equal(order.Id, result.Id);
        Assert.Equal(order.CustomerId, result.CustomerId);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsTotalAmountFromDomain()
    {
        var order = ExistingOrder();
        _repository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        Assert.Equal(380.00m, result.TotalAmount);
    }

    [Fact]
    public async Task Handle_WhenOrderExists_MapsItems()
    {
        var order = ExistingOrder();
        _repository.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _handler.Handle(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        var keyboard = Assert.Single(result.Items, i => i.ProductName == "Keyboard");
        Assert.Equal(2, keyboard.Quantity);
        Assert.Equal(150.00m, keyboard.UnitPrice);
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToRepository()
    {
        var order = ExistingOrder();
        using var cancellationSource = new CancellationTokenSource();
        var token = cancellationSource.Token;
        _repository.GetByIdAsync(order.Id, token).Returns(order);

        await _handler.Handle(new GetOrderByIdQuery(order.Id), token);

        await _repository.Received(1).GetByIdAsync(order.Id, token);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ThrowsNotFoundException()
    {
        var orderId = Guid.NewGuid();
        _repository.GetByIdAsync(orderId, Arg.Any<CancellationToken>()).Returns((Order?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new GetOrderByIdQuery(orderId), CancellationToken.None));
    }
}
