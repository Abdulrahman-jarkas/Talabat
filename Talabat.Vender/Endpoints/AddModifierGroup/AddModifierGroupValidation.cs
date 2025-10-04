using FluentValidation;
using Talabat.Vender.Endpoints.AddProduct;

namespace Talabat.Vender.Endpoints.AddModifierGroup;

public class AddModifierGroupValidation : AbstractValidator<AddModifierGroupRequest>
{
	public AddModifierGroupValidation()
	{
		RuleFor(p => p.Title)
			.NotEmpty()
			.WithMessage("Title is required.")
			.MinimumLength(3)
			.WithMessage("Title must be at least 3 characters long.")
			.MaximumLength(50)
			.WithMessage("Title must not exceed 50 characters.");

		RuleFor(p => p.Min)
			.GreaterThan(0)
			.WithMessage("Min must be greater than 0.");

		RuleFor(p => p.Max)
			.GreaterThan(0)
			.WithMessage("Min must be greater than 0.")
			.LessThanOrEqualTo(p => p.Data.Count)
			.WithMessage("Max must be less than or equal to the number of options.")
			.GreaterThanOrEqualTo(p => p.Min)
			.WithMessage("Max must be greater than or equal to Min.");


		RuleFor(p => p.Data)
			.NotNull()
			.WithMessage("Should have at least one modifier")
			.NotEmpty()
			.WithMessage("Should have at least one modifier")
			.Must(items => items.Select(i => i.ModifierId).Distinct().Count() == items.Count())
			.WithMessage("Duplicate Modifiers are not allowed.");

		RuleForEach(p => p.Data)
			.SetValidator(new ModifierGroupItemValidation());
	}
}

public class ModifierGroupItemValidation : AbstractValidator<AddModifierGroupRequest.ModifierGroupItem>
{
	public ModifierGroupItemValidation()
	{
		RuleFor(p => p.GroupIds)
			.Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
			.WithMessage("Duplicate group IDs are not allowed.");

		RuleForEach(p => p.GroupIds ?? new List<Guid>())
			.NotEqual(Guid.Empty)
			.WithMessage("Group ID must be a valid non-empty GUID.");
	}
}

