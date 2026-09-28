using FluentValidation.TestHelper;
using OrdersBackend.Application.Orders.Commands.CreateOrder;

namespace OrdersBackend.Application.Tests.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    private static CreateOrderCommand CommandWithItems(params CreateOrderItem[] items) =>
        new(Guid.NewGuid(), items);

    private static CreateOrderItem ValidItem() => new("Keyboard", 1, 150.00m);

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var result = _validator.TestValidate(CommandWithItems(ValidItem(), new CreateOrderItem("Mouse", 3, 0.01m)));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyCustomerId_HasErrorForCustomerId()
    {
        var result = _validator.TestValidate(new CreateOrderCommand(Guid.Empty, [ValidItem()]));

        result.ShouldHaveValidationErrorFor(c => c.CustomerId);
    }

    [Fact]
    public void Validate_WithoutItems_HasErrorForItems()
    {
        var result = _validator.TestValidate(CommandWithItems());

        result.ShouldHaveValidationErrorFor(c => c.Items);
    }

    [Fact]
    public void Validate_WithNullItems_HasErrorForItemsWithoutThrowing()
    {
        var result = _validator.TestValidate(new CreateOrderCommand(Guid.NewGuid(), null!));

        result.ShouldHaveValidationErrorFor(c => c.Items);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_HasErrorForItemQuantity(int quantity)
    {
        var result = _validator.TestValidate(CommandWithItems(new CreateOrderItem("Keyboard", quantity, 150.00m)));

        result.ShouldHaveValidationErrorFor("Items[0].Quantity");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Validate_WithNonPositiveUnitPrice_HasErrorForItemUnitPrice(decimal unitPrice)
    {
        var result = _validator.TestValidate(CommandWithItems(new CreateOrderItem("Keyboard", 1, unitPrice)));

        result.ShouldHaveValidationErrorFor("Items[0].UnitPrice");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithBlankProductName_HasErrorForItemProductName(string? productName)
    {
        var result = _validator.TestValidate(CommandWithItems(new CreateOrderItem(productName!, 1, 150.00m)));

        result.ShouldHaveValidationErrorFor("Items[0].ProductName");
    }

    [Fact]
    public void Validate_WithProductNameAtMaxLength_HasNoErrors()
    {
        var result = _validator.TestValidate(CommandWithItems(new CreateOrderItem(new string('a', 200), 1, 150.00m)));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithProductNameAboveMaxLength_HasErrorForItemProductName()
    {
        var result = _validator.TestValidate(CommandWithItems(new CreateOrderItem(new string('a', 201), 1, 150.00m)));

        result.ShouldHaveValidationErrorFor("Items[0].ProductName");
    }

    [Fact]
    public void Validate_WithSeveralInvalidItems_ReportsEachItemByIndex()
    {
        var result = _validator.TestValidate(CommandWithItems(
            ValidItem(),
            new CreateOrderItem("Mouse", 0, 80.00m),
            new CreateOrderItem("Cable", 1, 0m)));

        result.ShouldNotHaveValidationErrorFor("Items[0].Quantity");
        result.ShouldHaveValidationErrorFor("Items[1].Quantity");
        result.ShouldHaveValidationErrorFor("Items[2].UnitPrice");
    }
}
