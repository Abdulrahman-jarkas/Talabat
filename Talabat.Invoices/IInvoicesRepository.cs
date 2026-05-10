namespace Talabat.Invoices;

internal interface IInvoicesRepository
{
	Task<Customer?> GetCustomerAsync(Guid userId, CancellationToken cancellationToken = default);
	Task SaveChangesAsync();
}
