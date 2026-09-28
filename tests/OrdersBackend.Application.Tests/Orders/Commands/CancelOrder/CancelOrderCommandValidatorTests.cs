using FluentValidation.TestHelper;
using OrdersBackend.Application.Orders.Commands.CancelOrder;

namespace OrdersBackend.Application.Tests.Orders.Commands.CancelOrder;

public class CancelOrderCommandValidatorTests
{
    private readonly CancelOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidOrderId_HasNoErrors()
    {
        var result = _validator.TestValidate(new CancelOrderCommand(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasErrorForOrderId()
    {
        var result = _validator.TestValidate(new CancelOrderCommand(Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }
}
