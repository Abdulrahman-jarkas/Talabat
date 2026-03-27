using ErrorOr;

namespace Talabat.Checkout.Application.Services;

internal interface ICheckoutSessionService
{
	Task<ErrorOr<Success>> CreateAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default);
	Task<ErrorOr<(Guid PaymentId, string PaymentUrl)>> CheckoutAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default);
}
