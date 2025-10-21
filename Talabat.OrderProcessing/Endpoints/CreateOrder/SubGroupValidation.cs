using FluentValidation;
using Talabat.OrderProcessing.Endpoints.CreateOrder;
using Talabat.ProductsManagement.Contracts;

//public class SubGroupValidation : AbstractValidator<CreateOrderItemRequest.SubGroup>
//{
//	private readonly ModifierSubGroupResponse _subGroupDef;

//	public SubGroupValidation(ModifierSubGroupResponse subGroupDef)
//	{
//		_subGroupDef = subGroupDef;

//		RuleFor(sg => sg.Id)
//			.Equal(_subGroupDef.Id)
//			.WithMessage($"Invalid subgroup Id. Expected {_subGroupDef.Id}.");

//		RuleFor(sg => sg.Modifiers)
//			.NotEmpty()
//			.WithMessage("At least one modifier is required.")
//			.Must(m => m.Distinct().Count() == m.Count())
//			.WithMessage("Duplicate modifiers are not allowed.")
//			.Must(m => m.Count >= _subGroupDef.Min)
//			.WithMessage($"At least {_subGroupDef.Min} modifiers must be selected.")
//			.Must(m => m.Count <= _subGroupDef.Max)
//			.WithMessage($"No more than {_subGroupDef.Max} modifiers can be selected.");
//	}
//}
