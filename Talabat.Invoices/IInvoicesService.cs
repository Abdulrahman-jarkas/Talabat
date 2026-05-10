using ErrorOr;

namespace Talabat.Invoices;

internal interface IInvoicesService
{
	Task<ErrorOr<Invoice>> CreateInvoiceAsync();
}
