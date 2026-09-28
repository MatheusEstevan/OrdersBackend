using FluentValidation;

namespace OrdersBackend.Application.Orders.Queries.GetOrderById
{
    public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("OrderId é obrigatório.");
        }
    }
}
