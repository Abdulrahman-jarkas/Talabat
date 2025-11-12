using ErrorOr;
using FastEndpoints;

namespace Talabat.Taxes;

public class AddTaxPolicyEndpoint(ITaxesService taxesService) : Endpoint<AddTaxPolicyRequest, ErrorOr<Success>>
{
	public override void Configure()
	{
		Post("/api/taxes/create");
		AllowAnonymous();
	}

	public override Task HandleAsync(AddTaxPolicyRequest req, CancellationToken ct)
	{
		return taxesService.AddTaxPolicy(req, ct);
	}
}
