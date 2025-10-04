using FastEndpoints;
using Talabat.Vender.Application;

namespace Talabat.Vender.Endpoints.AddModifierGroup;

public class AddModifierGroupEndpoint(IProductService productService) : Endpoint<AddModifierGroupRequest, ModifierGroupDto>
{
	public override void Configure()
	{
		Post("/api/modifierGroups");
		AllowAnonymous();
	}

	public override async Task HandleAsync(AddModifierGroupRequest req, CancellationToken ct)
	{
		await productService.AddModifierGroup(req.ToDto());
		//await Send.CreatedAtAsync(nameof(GetProductEndpoint), new { group });
	}
}