using FluentValidation.TestHelper;
using OrdersBackend.Application.Orders.Queries.GetAllOrders;

namespace OrdersBackend.Application.Tests.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryValidatorTests
{
    private readonly GetAllOrdersQueryValidator _validator = new();

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 10)]
    [InlineData(5, 100)]
    public void Validate_WithValidPagination_HasNoErrors(int page, int pageSize)
    {
        var result = _validator.TestValidate(new GetAllOrdersQuery(page, pageSize));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositivePage_HasErrorForPage(int page)
    {
        var result = _validator.TestValidate(new GetAllOrdersQuery(page, 10));

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_HasErrorForPageSize(int pageSize)
    {
        var result = _validator.TestValidate(new GetAllOrdersQuery(1, pageSize));

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
