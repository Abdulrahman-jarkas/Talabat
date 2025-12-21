using ErrorOr;
using MediatR;

namespace Talabat.Invoices.Contracts;

public record ActiveInvoiceQuery(Guid CustomerId) : IRequest<ErrorOr<ActiveInvoiceResponse>>;

public record ActiveInvoiceResponse(Guid InvoiceId, Guid MerchantId, IReadOnlyList<InvoiceItemResponse> InvoiceItems)
{
	public decimal TotalPrice => InvoiceItems.Sum(ii => ii.BasePrice * ii.Quantity);
}

public record InvoiceItemResponse(Guid ProductId, int Quantity, decimal BasePrice);
