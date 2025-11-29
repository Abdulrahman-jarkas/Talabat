using MediatR;

namespace Talabat.Taxes.Contracts;

public record GetTaxQuery(int TaxId) : IRequest<TaxResponse?>;

public record TaxResponse(int Id, decimal VatPercentage, string Name);

public record GetServiceFeesQuery(int countryId) : IRequest<ServiceFeesResponse?>;

public record ServiceFeesResponse(decimal Value, decimal VatPercentage);

