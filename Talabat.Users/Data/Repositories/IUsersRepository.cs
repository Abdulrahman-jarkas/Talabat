using Talabat.Users.Domain.CustomerAggregate;

namespace Talabat.Users.Data.Repositories;

internal interface IUsersRepository
{
	Task<Customer?> GetCustomerAsync(Guid customerId);
	Task<Customer?> GetCustomerDetailsAsync(Guid customerId, CancellationToken cancellationToken);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
