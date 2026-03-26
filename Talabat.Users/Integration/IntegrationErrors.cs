using ErrorOr;

namespace Talabat.Users.Integration;

internal static class IntegrationErrors
{
	public static class OnOrderPlaced
	{
		public static Error CustomerNotFound(Guid checkoutSessionId) =>
			Error.NotFound(
				"OnOrderPlaced.CustomerNotFound",
				$"Customer not found for checkout session '{checkoutSessionId}'.");

		public static Error CustomerIdMismatch(Guid expectedCustomerId, Guid actualCustomerId) =>
			Error.Validation(
				"OnOrderPlaced.CustomerIdMismatch",
				$"Security violation: Customer ID mismatch. Expected '{expectedCustomerId}', but got '{actualCustomerId}'.");

		public static Error FailedToSetOrderId(Guid checkoutSessionId, List<Error> errors) =>
			Error.Failure(
				"OnOrderPlaced.FailedToSetOrderId",
				$"Failed to set order ID for checkout session '{checkoutSessionId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");
	}

	public static class PaymentSuccessed
	{
		public static Error CustomerNotFound(Guid customerId) =>
			Error.NotFound(
				"PaymentSuccessed.CustomerNotFound",
				$"Customer '{customerId}' not found for payment completion.");

		public static Error FailedToSetPaymentId(Guid customerId, List<Error> errors) =>
			Error.Failure(
				"PaymentSuccessed.FailedToSetPaymentId",
				$"Failed to set payment ID for customer '{customerId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");

		public static Error FailedToCompleteCheckout(Guid customerId, List<Error> errors) =>
			Error.Failure(
				"PaymentSuccessed.FailedToCompleteCheckout",
				$"Failed to complete checkout for customer '{customerId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");
	}

	public static class PaymentFailed
	{
		public static Error CustomerNotFound(Guid customerId) =>
			Error.NotFound(
				"PaymentFailed.CustomerNotFound",
				$"Customer '{customerId}' not found for payment cancellation.");

		public static Error FailedToCancelCheckout(Guid customerId, List<Error> errors) =>
			Error.Failure(
				"PaymentFailed.FailedToCancelCheckout",
				$"Failed to cancel checkout for customer '{customerId}'. Errors: {string.Join(", ", errors.Select(e => e.Description))}");
	}
}
