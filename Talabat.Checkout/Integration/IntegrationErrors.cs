using ErrorOr;

namespace Talabat.Checkout.Integration;

internal static class IntegrationErrors
{
	public static class PaymentSuccessed
	{
		public static Error CheckoutSessionNotFound(Guid customerId) =>
			Error.NotFound(
				"PaymentSuccessed.CheckoutSessionNotFound",
				$"Active checkout session not found for customer '{customerId}'.");

		public static Error FailedToCreateOrder(Guid checkoutSessionId, List<Error> errors) =>
			Error.Failure(
				"PaymentSuccessed.FailedToCreateOrder",
				$"Failed to create order for checkout session '{checkoutSessionId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");

		public static Error FailedToComplete(Guid checkoutSessionId, List<Error> errors) =>
			Error.Failure(
				"PaymentSuccessed.FailedToComplete",
				$"Failed to complete checkout session '{checkoutSessionId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");
	}

	public static class PaymentFailed
	{
		public static Error CheckoutSessionNotFound(Guid customerId) =>
			Error.NotFound(
				"PaymentFailed.CheckoutSessionNotFound",
				$"Active checkout session not found for customer '{customerId}'.");

		public static Error FailedToCancel(Guid checkoutSessionId, List<Error> errors) =>
			Error.Failure(
				"PaymentFailed.FailedToCancel",
				$"Failed to cancel checkout session '{checkoutSessionId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");
	}
}
