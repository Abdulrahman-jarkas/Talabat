using ErrorOr;

namespace Talabat.Users.Domain.CustomerAggregate.Checkout;

internal record CheckoutSessionErrors
{
	public static Error QuantityMismatch =>
			Error.Conflict("CheckoutSession.QuantityMismatch", "The quantity of items in the checkout session does not match the cart.");

	public static Error PriceMismatch =>
			Error.Conflict("CheckoutSession.PriceMismatch", "The price of items in the checkout session does not match the cart.");
}
