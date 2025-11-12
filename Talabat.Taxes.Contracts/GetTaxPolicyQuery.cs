using MediatR;

namespace Talabat.Taxes.Contracts;

public record GetTaxPolicyQuery(string CountryCode) : IRequest<TaxPolicyResponse?>;

public record TaxPolicyResponse(int Id, string CountryCode, List<TaxCategoryResponse> Categories);

public record TaxCategoryResponse(int Id, string  Name, decimal vat);
