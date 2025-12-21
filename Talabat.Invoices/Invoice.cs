using Ardalis.GuardClauses;
using ErrorOr;
using Talabat.SharedKernal;

namespace Talabat.Invoices;

internal class Invoice : AggregateRoot
{
	private readonly List<InvoiceItem> _items = new();
	public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

	public Guid MerchantId { get; }
	public Guid UserId { get; }
	public InvoiceStatus Status { get; private set; }

	public Invoice(Guid userId, Guid merchantId, IReadOnlyList<InvoiceItem> invoiceItems)
	{
		UserId = Guard.Against.Default(userId, nameof(userId));
		MerchantId = Guard.Against.Default(merchantId, nameof(merchantId));
		Guard.Against.NullOrEmpty(invoiceItems, nameof(invoiceItems));
		Status = InvoiceStatus.Active;

		_items.AddRange(invoiceItems);
	}

	public ErrorOr<Success> Cancel()
	{
		if (Status != InvoiceStatus.Active)
			return Error.Failure("Invoice.NotActive", "Only active invoices can be inactivated.");

		 Status = InvoiceStatus.Cancelled;
		return Result.Success;
	}

	public ErrorOr<Success> Complete()
	{
		if (Status != InvoiceStatus.Active)
			return Error.Failure("Invoice.NotActive", "Only active invoices can be completed.");

		Status = InvoiceStatus.Completed;
		return Result.Success;
	}

	private Invoice()
	{
		// EF
	}
}