using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Repositories;

internal interface IUsersRepository
{
	Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
	Task<Customer?> GetCustomerByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
	Task<Customer?> GetCustomerWithAddressesByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
	Task<List<Customer>?> GetCustomersWithProductInCartAsync(Guid productId, CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
