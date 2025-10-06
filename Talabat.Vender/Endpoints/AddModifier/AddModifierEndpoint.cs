using FastEndpoints;
using Talabat.Vender.Application;

namespace Talabat.Vender.Endpoints.AddModifier;

public class AddModifierEndpoint(IProductService productService) : Endpoint<AddModifierRequest, ModifierDto>
{
	public override void Configure()
	{
		Post("/api/modifiers");
		AllowAnonymous();
	}

	public override async Task HandleAsync(AddModifierRequest req, CancellationToken ct)
	{
		await productService.AddModifier(req.ToDto());
		await Send.OkAsync();
	}
}