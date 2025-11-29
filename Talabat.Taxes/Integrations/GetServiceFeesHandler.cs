using MediatR;
using Talabat.Taxes.Contracts;
using Talabat.Taxes.Repositories;

namespace Talabat.Taxes.Integrations;

public class GetServiceFeesHandler(ITaxesRepository taxesRepository) : IRequestHandler<GetServiceFeesQuery, ServiceFeesResponse?>
{
	public async Task<ServiceFeesResponse?> Handle(GetServiceFeesQuery request, CancellationToken cancellationToken)
	{
		var country = await taxesRepository.GetCountry(request.countryId, cancellationToken);

		var tax = await taxesRepository.GetTax(country.ServiceFees.TaxId, cancellationToken);

		if(country == null || tax == null) return null;

		return new ServiceFeesResponse(country.ServiceFees.Value, tax.VatPercentage);
	}
}