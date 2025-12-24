using ErrorOr;
using MediatR;
using Talabat.Payments.Contracts;

namespace Talabat.Payments.Integrations;

public class CreatePaymentSessionHandler(IPaymentService paymentService) : IRequestHandler<CreatePaymentSessionRequest, ErrorOr<CreatePaymentSessionResponseDto>>
{
	public async Task<ErrorOr<CreatePaymentSessionResponseDto>> Handle(CreatePaymentSessionRequest request, CancellationToken cancellationToken)
	{
		var result = await paymentService.CreatePaymentSession(request.CustomerId, request.CheckoutSessionId, request.Amount);

		if(result.IsError)
		{
			return result.Errors;
		}

		var response = result.Value;
		var responseDto = new CreatePaymentSessionResponseDto
		{
			PaymentId = response.PaymentId,
			PaymentUrl = response.PaymentUrl
		};

		return responseDto;
	}
}
