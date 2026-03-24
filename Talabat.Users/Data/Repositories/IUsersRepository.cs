using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Repositories;

internal interface IUsersRepository
{
	Task<Customer?> GetCustomerAsync(Guid customerId);
	Task<Customer?> GetCustomerDetailsAsync(Guid customerId, CancellationToken cancellationToken);
	Task<List<Customer>?> GetCustomersWithProductInCartAsync(Guid productId, CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
