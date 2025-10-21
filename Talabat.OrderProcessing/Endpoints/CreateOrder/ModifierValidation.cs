using FluentValidation;
using Talabat.OrderProcessing.Endpoints.CreateOrder;
using Talabat.ProductsManagement.Contracts;

public class ModifierValidation : AbstractValidator<CreateOrderItemRequest.Modifier>
{
	public ModifierValidation()
	{

		RuleFor(m => m.Id)
			.NotNull()
			.WithMessage("Modifier Id is required")
			.NotEmpty()
			.WithMessage("Modifier Id is required");

		//RuleFor(m => m.SubGroups)
		//	.Must(sg => sg.Select(x => x.Id).Distinct().Count() == sg.Count())
		//	.WithMessage("Duplicate subgroups are not allowed.");

		//RuleForEach(m => m.SubGroups)
		//	.CustomAsync(ValidateSubGroupAsync);
	}

	//private async Task ValidateSubGroupAsync(CreateOrderItemRequest.SubGroup subGroup, ValidationContext<CreateOrderItemRequest.Modifier> context, CancellationToken token)
	//{
	//	var subDef = _modifierDef.ModifierSubGroups.FirstOrDefault(s => s.Id == subGroup.Id);
	//	if (subDef == null)
	//	{
	//		context.AddFailure($"SubGroup[{subGroup.Id}]", $"SubGroup with ID {subGroup.Id} not found in modifier {_modifierDef.Id}.");
	//		return;
	//	}

	//	var validator = new SubGroupValidation(subDef);
	//	var result = await validator.ValidateAsync(subGroup, token);

	//	foreach (var failure in result.Errors)
	//		context.AddFailure($"SubGroup[{subGroup.Id}].{failure.PropertyName}", failure.ErrorMessage);
	//}
}
