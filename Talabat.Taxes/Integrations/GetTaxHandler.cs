using MediatR;
using Talabat.Taxes.Contracts;
using Talabat.Taxes.Repositories;

namespace Talabat.Taxes.Integrations;

public class GetTaxHandler(ITaxesRepository taxesRepository) : IRequestHandler<GetTaxQuery, TaxResponse?>
{
	public async Task<TaxResponse?> Handle(GetTaxQuery request, CancellationToken cancellationToken)
	{
		var res = await taxesRepository.GetTax(request.TaxId, cancellationToken);
		if(res == null) 
			return null;

		return res.ToResponse();
	}
}