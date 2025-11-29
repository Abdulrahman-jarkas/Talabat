using ErrorOr;
using FastEndpoints;

namespace Talabat.Payments.Endpoints;

public enum PaymentEventType
{
	PaymentSucceeded,
	PaymentFailed,
	PaymentRefunded,
	PaymentRefundFailed
}

public class PaymentWebHookRequest
{
	public PaymentEventType EventType { get; set; }
	public string OrderId { get; set; }
	public string PaymentId { get; set; }
	public decimal Amount { get; set; }
	public bool isSuccess { get; set; }
}

public class PaymentWebHook(IPaymentService paymentService) : Endpoint<PaymentWebHookRequest>
{
	public override void Configure()
	{
		Post("/api/payments/webhook");
		AllowAnonymous();
	}

	public override async Task HandleAsync(PaymentWebHookRequest req, CancellationToken ct)
	{
		// here we need to handle idempotency to avoid processing the same event multiple times

		var paymentId = Guid.Parse(req.PaymentId);

		// Execute the appropriate handler
		var task = req.EventType switch
		{
			PaymentEventType.PaymentSucceeded => paymentService.OnPaymentSuccess(paymentId),
			PaymentEventType.PaymentFailed => paymentService.OnPaymentFailed(paymentId),
			PaymentEventType.PaymentRefunded => paymentService.OnPaymentRefundSuccess(paymentId),
			PaymentEventType.PaymentRefundFailed => paymentService.OnPaymentRefundFailed(paymentId),
			_ => Task.FromResult<ErrorOr<Success>>(Error.Validation(
					code: "Payment.InvalidEventType",
					description: $"Invalid payment event type {req.EventType.ToString()}."
				))
		};


		var result = await task;

		// Unified response handling
		if (!result.IsError)
		{
			await Send.OkAsync();
		}
		else
		{
			// Tell the payment gateway to retry later
			await Send.ErrorsAsync();
		}
	}
}
