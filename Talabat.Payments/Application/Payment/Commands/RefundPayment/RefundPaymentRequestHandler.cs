using ErrorOr;
using MediatR;
using Talabat.Payments.Contracts;

namespace Talabat.Payments.Application.Payment.Commands.RefundPayment;

public class RefundPaymentRequestHandler(IPaymentService paymentService)
	: IRequestHandler<RefundPaymentRequest, ErrorOr<Success>>
{
	public async Task<ErrorOr<Success>> Handle(RefundPaymentRequest request, CancellationToken cancellationToken)
	{
		return await paymentService.Refund(request.PaymentId);
	}
}
