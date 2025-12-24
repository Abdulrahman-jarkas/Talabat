using ErrorOr;

namespace Talabat.Payments
{
	public interface IPaymentService
	{
		Task<ErrorOr<CreatePaymentSessionResponse>> CreatePaymentSession(Guid customerId, Guid checkoutSessionId, decimal amount);
		Task<ErrorOr<Success>> Refund(Guid paymentId);
		Task<ErrorOr<Success>> OnPaymentSuccess(Guid paymentId);
		Task<ErrorOr<Success>> OnPaymentFailed(Guid paymentId);
		Task<ErrorOr<Success>> OnPaymentRefundSuccess(Guid paymentId);
		Task<ErrorOr<Success>> OnPaymentRefundFailed(Guid paymentId);
	}
}