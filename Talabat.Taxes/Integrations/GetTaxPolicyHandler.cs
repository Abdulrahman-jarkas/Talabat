using MediatR;
using Talabat.Taxes.Contracts;

namespace Talabat.Taxes.Integrations;

public class GetTaxPolicyHandler(ITaxesService taxesService) : IRequestHandler<GetTaxPolicyQuery, TaxPolicyResponse?>
{
	public async Task<TaxPolicyResponse?> Handle(GetTaxPolicyQuery request, CancellationToken cancellationToken)
	{
		var res = await taxesService.GetTaxPolicy(request.CountryCode);
		if(res == null) 
			return null;

		return res.ToResponse();
	}
}