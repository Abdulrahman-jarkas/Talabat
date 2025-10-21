using FluentValidation;
using Talabat.OrderProcessing.Endpoints.CreateOrder;

public class OrderItemValidation : AbstractValidator<CreateOrderItemRequest>
{

	public OrderItemValidation()
	{
		RuleFor(i => i.ProductId)
			.NotEmpty()
			.WithMessage("ProductId is required");

		RuleFor(i => i.Quantity)
			.GreaterThan(0);

		RuleFor(m => m.Groups)
			.Must(g => g.Select(x => x.Id).Distinct().Count() == g.Count())
			.WithMessage("Groups should not be duplicated");
	}
}