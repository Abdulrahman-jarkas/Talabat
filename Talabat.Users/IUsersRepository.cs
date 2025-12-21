namespace Talabat.Users;

internal interface IUsersRepository
{
	Task<Customer?> GetCustomerAsync(Guid customerId);
	Task<Customer?> GetCustomerDetailsAsync(Guid customerId, CancellationToken cancellationToken);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
