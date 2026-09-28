using FluentValidation.TestHelper;
using OrdersBackend.Application.Orders.Queries.GetOrderById;

namespace OrdersBackend.Application.Tests.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryValidatorTests
{
    private readonly GetOrderByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidOrderId_HasNoErrors()
    {
        var result = _validator.TestValidate(new GetOrderByIdQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyOrderId_HasErrorForOrderId()
    {
        var result = _validator.TestValidate(new GetOrderByIdQuery(Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }
}
