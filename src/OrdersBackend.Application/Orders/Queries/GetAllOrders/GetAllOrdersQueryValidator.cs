using FluentValidation;

namespace OrdersBackend.Application.Orders.Queries.GetAllOrders
{
    public sealed class GetAllOrdersQueryValidator : AbstractValidator<GetAllOrdersQuery>
    {
        public GetAllOrdersQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Página deve ser maior que zero.");
            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100.");
        }
    }
}
