using ErrorOr;

namespace Talabat.Users;

internal interface ICustomerService
{
	Task<ErrorOr<Success>> AddCartItemAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
	Task<ErrorOr<Success>> RemoveCartItemAsync(Guid productId, CancellationToken cancellationToken = default);
}
