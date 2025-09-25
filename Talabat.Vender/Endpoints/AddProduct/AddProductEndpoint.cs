using FastEndpoints;
using Talabat.Vender.Mappers;

namespace Talabat.Vender.Endpoints.AddProduct;

public class AddProductEndpoint : Endpoint<AddProductRequest>
{
	public override void Configure()
	{
		Post("/api/products");
		AllowAnonymous();
	}

	public override Task HandleAsync(AddProductRequest req, CancellationToken ct)
	{
		var product = req.ToDto();
		return base.HandleAsync(req, ct);
	}
}
