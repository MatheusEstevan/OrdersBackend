using NSubstitute;
using OrdersBackend.Application.Abstractions;
using OrdersBackend.Application.Orders.Queries.GetAllOrders;
using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Application.Tests.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandlerTests
{
    private static readonly DateTime CreatedAt = new(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

    private readonly IOrderRepository _repository = Substitute.For<IOrderRepository>();
    private readonly GetAllOrdersQueryHandler _handler;

    public GetAllOrdersQueryHandlerTests()
    {
        _handler = new GetAllOrdersQueryHandler(_repository);
    }

    private static Order NewOrder(decimal unitPrice) =>
        Order.Create(Guid.NewGuid(), CreatedAt, [("Keyboard", 1, unitPrice)]);

    private void RepositoryReturns(int totalCount, params Order[] orders)
    {
        _repository
            .ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(orders);
        _repository
            .CountAsync(Arg.Any<CancellationToken>())
            .Returns(totalCount);
    }

    [Fact]
    public async Task Handle_ReturnsOrdersMappedToDtosInRepositoryOrder()
    {
        var first = NewOrder(100.00m);
        var second = NewOrder(200.00m);
        RepositoryReturns(2, first, second);

        var result = await _handler.Handle(new GetAllOrdersQuery(1, 10), CancellationToken.None);

        Assert.Collection(result.Items,
            dto => { Assert.Equal(first.Id, dto.Id); Assert.Equal(100.00m, dto.TotalAmount); },
            dto => { Assert.Equal(second.Id, dto.Id); Assert.Equal(200.00m, dto.TotalAmount); });
    }

    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(25, 10, 3)]
    public async Task Handle_CalculatesTotalPagesRoundingUp(int totalCount, int pageSize, int expectedTotalPages)
    {
        RepositoryReturns(totalCount);

        var result = await _handler.Handle(new GetAllOrdersQuery(1, pageSize), CancellationToken.None);

        Assert.Equal(expectedTotalPages, result.TotalPages);
    }

    [Fact]
    public async Task Handle_PassesPaginationAndCancellationTokenToRepository()
    {
        RepositoryReturns(0);
        using var cancellationSource = new CancellationTokenSource();
        var token = cancellationSource.Token;

        await _handler.Handle(new GetAllOrdersQuery(3, 20), token);

        await _repository.Received(1).ListAsync(3, 20, token);
        await _repository.Received(1).CountAsync(token);
    }

    [Fact]
    public async Task Handle_WhenThereAreNoOrders_ReturnsEmptyList()
    {
        RepositoryReturns(0);

        var result = await _handler.Handle(new GetAllOrdersQuery(1, 10), CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalPages);
    }
}
