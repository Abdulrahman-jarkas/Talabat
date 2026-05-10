using ErrorOr;
using MediatR;
using Talabat.Invoices.Contracts;

namespace Talabat.Invoices.Integration;

internal class ActiveInvoiceQueryHandler(IInvoicesRepository invoicesRepository) : IRequestHandler<ActiveInvoiceQuery, ErrorOr<ActiveInvoiceResponse>>
{
	public async Task<ErrorOr<ActiveInvoiceResponse>> Handle(ActiveInvoiceQuery request, CancellationToken cancellationToken)
	{
		var res = await invoicesRepository.GetCustomerAsync(request.CustomerId, cancellationToken);

		if (res is null)
			return Error.NotFound(description: "Customer not found.");

		var activeInvoiceResult = res.GetActiveInvoice();

		if (activeInvoiceResult.IsError)
			return activeInvoiceResult.Errors;

		return new ActiveInvoiceResponse(
			activeInvoiceResult.Value.Id,
			activeInvoiceResult.Value.MerchantId,
			activeInvoiceResult.Value.Items
				.Select(i => new InvoiceItemResponse(i.ProductId, i.Quantity, i.BasePrice))
				.ToList()
				.AsReadOnly());

	}
}
