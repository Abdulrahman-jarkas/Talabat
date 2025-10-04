using FluentValidation;
using Talabat.Vender.Endpoints.AddProduct;

namespace Talabat.Vender.Endpoints.AddModifier;

public class AddModifierValidation : AbstractValidator<AddModifierRequest>
{
	public AddModifierValidation()
	{
		RuleFor(p => p.Title)
			.NotEmpty()
			.WithMessage("Title is required.")
			.MinimumLength(3)
			.WithMessage("Title must be at least 3 characters long.")
			.MaximumLength(50)
			.WithMessage("Title must not exceed 50 characters.");

		RuleFor(p => p.Price)
			.GreaterThan(0)
			.WithMessage("Price must be greater than 0.")
			.LessThanOrEqualTo(1000000)
			.WithMessage("Price must not exceed 1,000,000.")
			.PrecisionScale(18, 2, false)
			.WithMessage("Price must have no more than 2 decimal places.");
	}
}

