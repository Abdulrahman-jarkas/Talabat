using ErrorOr;
using Talabat.Users.Domain.CustomerAggregate.Checkout;

namespace Talabat.Users;

internal interface ICustomerService
{
	Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> Checkout(Guid addressId, PaymentType paymentType, CancellationToken cancellationToken = default);
	Task<ErrorOr<Success>> CreateCheckoutSession(CancellationToken cancellationToken = default);
	Task<ErrorOr<Success>> CancelCheckoutSession(CancellationToken cancellationToken = default);
	Task<ErrorOr<Success>> AddCartItemAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
	Task<ErrorOr<Success>> RemoveCartItemAsync(Guid productId, CancellationToken cancellationToken = default);
}
