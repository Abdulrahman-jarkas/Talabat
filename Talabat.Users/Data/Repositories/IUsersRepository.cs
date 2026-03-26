using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Repositories;

internal interface IUsersRepository
{
	Task<Customer?> GetCustomerWithActiveCheckoutAsync(Guid customerId, CancellationToken cancellationToken = default);
	Task<Customer?> GetCustomerWithAddressesAsync(Guid customerId, CancellationToken cancellationToken = default);
	Task<Customer?> GetCustomerByCheckoutSessionIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken);
	Task<List<Customer>?> GetCustomersWithProductInCartAsync(Guid productId, CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
