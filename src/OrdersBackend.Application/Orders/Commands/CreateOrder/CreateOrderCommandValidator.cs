using FluentValidation;

namespace OrdersBackend.Application.Orders.Commands.CreateOrder
{
   public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId é obrigatório");
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("É necessário possuir pelo menos um item.");
            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantidade deve ser maior que zero.");
                item.RuleFor(i => i.ProductName).NotEmpty().WithMessage("Nome do produto não pode ser vazio.").MaximumLength(200).WithMessage("Nome do produto inválido.");
                item.RuleFor(i => i.UnitPrice).GreaterThan(0).WithMessage("Preço por unidade deve ser maior que 0.");
            });
        }
    }
}
