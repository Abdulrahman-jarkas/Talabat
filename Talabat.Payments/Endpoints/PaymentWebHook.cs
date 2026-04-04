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
	public Guid PaymentId { get; set; }
	public decimal Amount { get; set; }
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

		// Execute the appropriate handler
		var task = req.EventType switch
		{
			PaymentEventType.PaymentSucceeded => paymentService.OnPaymentSuccess(req.PaymentId),
			PaymentEventType.PaymentFailed => paymentService.OnPaymentFailed(req.PaymentId),
			PaymentEventType.PaymentRefunded => paymentService.OnPaymentRefundSuccess(req.PaymentId),
			PaymentEventType.PaymentRefundFailed => paymentService.OnPaymentRefundFailed(req.PaymentId),
			_ => Task.FromResult<ErrorOr<Success>>(Error.Validation(
					code: "Payment.InvalidEventType",
					description: $"Invalid payment event type {req.EventType}."))
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
