using FluentValidation;
using Talabat.OrderProcessing.Endpoints.CreateOrder;

public class GroupValidation : AbstractValidator<CreateOrderItemRequest.Group>
{
	public GroupValidation()
	{
		RuleFor(g => g.Id)
			.NotEmpty()
			.WithMessage("Group Id is required")
			.NotNull()
			.WithMessage("Group Id cannot be null");

		RuleFor(g => g.Modifiers)
			.Must(m => m.Select(x => x.Id).Distinct().Count() == m.Count())
			.WithMessage("Duplicate modifiers are not allowed.");

		RuleForEach(g => g.Modifiers)
			.NotNull()
			.WithMessage("Modifier cannot be null")
			.NotEmpty()
			.WithMessage("Modifier cannot be empty");
	}
}
