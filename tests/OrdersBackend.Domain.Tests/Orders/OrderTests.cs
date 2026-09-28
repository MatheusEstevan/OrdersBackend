using OrdersBackend.Domain.Common;
using OrdersBackend.Domain.Orders;

namespace OrdersBackend.Domain.Tests.Orders;

public class OrderTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly DateTime CreatedAt = new(2026, 1, 15, 10, 30, 0, DateTimeKind.Utc);

    private static Order CreateValidOrder(params (string ProductName, int Quantity, decimal UnitPrice)[] items)
    {
        if (items.Length == 0)
        {
            items = [("Keyboard", 1, 150.00m)];
        }

        return Order.Create(CustomerId, CreatedAt, items);
    }

    [Fact]
    public void Create_WithValidData_ReturnsPendingOrder()
    {
        var order = CreateValidOrder();

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(CustomerId, order.CustomerId);
        Assert.Equal(CreatedAt, order.CreatedAt);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void Create_WithValidItems_AddsItemsLinkedToOrder()
    {
        var order = CreateValidOrder(("Keyboard", 2, 150.00m), ("Mouse", 1, 80.00m));

        Assert.Equal(2, order.Items.Count);
        Assert.All(order.Items, item =>
        {
            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(order.Id, item.OrderId);
        });

        var keyboard = Assert.Single(order.Items, i => i.ProductName == "Keyboard");
        Assert.Equal(2, keyboard.Quantity);
        Assert.Equal(150.00m, keyboard.UnitPrice);
    }

    [Fact]
    public void Create_WithMultipleItems_GeneratesDistinctItemIds()
    {
        var order = CreateValidOrder(("Keyboard", 1, 150.00m), ("Mouse", 1, 80.00m));

        Assert.Equal(order.Items.Count, order.Items.Select(i => i.Id).Distinct().Count());
    }

    [Fact]
    public void Create_EachCall_GeneratesDistinctOrderIds()
    {
        var first = CreateValidOrder();
        var second = CreateValidOrder();

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Order.Create(Guid.Empty, CreatedAt, [("Keyboard", 1, 150.00m)]));
    }

    [Fact]
    public void Create_WithoutItems_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Order.Create(CustomerId, CreatedAt, []));
    }

    [Fact]
    public void Create_WithNullItems_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Order.Create(CustomerId, CreatedAt, null!));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositiveQuantity_ThrowsDomainException(int quantity)
    {
        Assert.Throws<DomainException>(() =>
            Order.Create(CustomerId, CreatedAt, [("Keyboard", quantity, 150.00m)]));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-10)]
    public void Create_WithNonPositiveUnitPrice_ThrowsDomainException(decimal unitPrice)
    {
        Assert.Throws<DomainException>(() =>
            Order.Create(CustomerId, CreatedAt, [("Keyboard", 1, unitPrice)]));
    }

    [Fact]
    public void Create_WhenAnyItemIsInvalid_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Order.Create(CustomerId, CreatedAt, [("Keyboard", 1, 150.00m), ("Mouse", 0, 80.00m)]));
    }

    [Fact]
    public void Items_CannotBeModifiedFromOutside()
    {
        var order = CreateValidOrder();
        var items = (ICollection<OrderItem>)order.Items;

        Assert.True(items.IsReadOnly);
        Assert.Throws<NotSupportedException>(() => items.Clear());
    }

    [Fact]
    public void TotalAmount_WithSingleItem_ReturnsQuantityTimesUnitPrice()
    {
        var order = CreateValidOrder(("Keyboard", 3, 150.00m));

        Assert.Equal(450.00m, order.TotalAmount);
    }

    [Fact]
    public void TotalAmount_WithMultipleItems_ReturnsSumOfAllItems()
    {
        var order = CreateValidOrder(("Keyboard", 2, 10.50m), ("Mouse", 1, 5.00m), ("Cable", 4, 0.99m));

        Assert.Equal(29.96m, order.TotalAmount);
    }

    [Fact]
    public void Cancel_WhenPending_ChangesStatusToCancelled()
    {
        var order = CreateValidOrder();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainExceptionAndKeepsStatus()
    {
        var order = CreateValidOrder();
        order.Cancel();

        Assert.Throws<DomainException>(order.Cancel);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }
}
