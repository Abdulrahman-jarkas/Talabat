using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Invoices;

internal class Customer : Entity
{
	public List<Invoice> _invoices { get; private set; } = new();
	public IReadOnlyList<Invoice> Invoices => _invoices.AsReadOnly();

	public Customer(Guid id)
		: base(id)
	{
	}

    public ErrorOr<Invoice> GetActiveInvoice()
	{
		var activeInvoicesCount = _invoices.Count(i => i.Status == InvoiceStatus.Active);
		if (activeInvoicesCount > 1)
			return Error.Failure("User.MultipleActiveInvoices", "The user has multiple active invoices.");


		var activeInvoice = _invoices.FirstOrDefault(i => i.Status == InvoiceStatus.Active);
		if (activeInvoice is null)
			return Error.NotFound("User.NoActiveInvoice", "The user has no active invoice.");

		return activeInvoice;
	}

	public ErrorOr<Success> AddInvoice(Invoice invoice)
	{
		var activeInvoiceResult = GetActiveInvoice();
		if (activeInvoiceResult.IsError == false)
		{
			var cancelResult = activeInvoiceResult.Value.Cancel();

			if (cancelResult.IsError)
				return cancelResult;
		}

		_invoices.Add(invoice);

		return Result.Success;
	}

	private Customer()
	{
		// EF
	}
}