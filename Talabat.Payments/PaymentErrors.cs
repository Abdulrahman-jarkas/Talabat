using ErrorOr;

namespace Talabat.Payments;

internal static class PaymentErrors
{
	public static Error NotFound(Guid paymentId) =>
		Error.NotFound(
			"Payment.NotFound",
			$"Payment with id {paymentId} was not found.");

	public static Error InvalidStatus(Guid paymentId, PaymentStatus status) =>
		Error.Conflict(
			"Payment.InvalidStatus",
			$"Payment with id {paymentId} has invalid status {status}.");
}
