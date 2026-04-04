using ErrorOr;

namespace Talabat.Users.Domain.CustomerAggregate;

public record CustomerErrors
{
	public static Error CartNotFound =>
		Error.NotFound("Customer.CartNotFound", "The cart was not found for the customer.");

	public static Error CartEmpty =>
		Error.Validation("Customer.CartEmpty", "The cart is empty.");

	public static Error MerchantMismatch =>
		Error.Conflict("Customer.MerchantMismatch", "The cart cannot belong to different merchants.");

	public static Error CustomerNotFound =>
		Error.NotFound("Customer.NotFound", "The customer was not found.");
}