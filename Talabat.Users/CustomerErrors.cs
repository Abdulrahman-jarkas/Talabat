using ErrorOr;

namespace Talabat.Users;

public record CustomerErrors
{
	public static Error UpdateCartWithActiveCheckoutSession =>
		Error.Failure("Customer.ActiveCheckoutSessionExists", "Cannot modify cart while there is an active checkout session.");

	public static Error ActiveCheckoutSessionExists =>
		Error.Failure("Customer.ActiveCheckoutSessionExists", "There is already an active checkout session for the customer.");

	public static Error NoActiveCheckoutSession =>
		Error.Failure("Customer.NoActiveCheckoutSession", "There is no active checkout session for the customer.");

	public static Error AddressNotFound =>
		Error.NotFound("Customer.AddressNotFound", "The specified address was not found for the customer.");

	public static Error CartNotFound =>
		Error.NotFound("Customer.CartNotFound", "The cart was not found for the customer.");

	public static Error CartEmpty =>
		Error.Failure("Customer.CartEmpty", "The cart is empty.");

	public static Error MerchantMismatch =>
		Error.Failure("Customer.MerchantMismatch", "The cart cannot belong to different merchants.");

	public static Error CustomerNotFound =>
		Error.NotFound("Customer.NotFound", "The customer was not found.");
}