using ErrorOr;
using Talabat.OrderProcessing.Application.DTOs;
using Talabat.OrderProcessing.Domain.CheckoutSessionAggregate;
using Talabat.OrderProcessing.Endpoints.CreateOrder;
using Talabat.ProductsManagement.Contracts;

namespace Talabat.OrderProcessing.Application.Services
{
	public interface IOrderService
	{
		Task<ErrorOr<OrderDetailsDto>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
		Task<ErrorOr<string>> StartCheckoutSession(CreateOrderRequest request, CancellationToken cancellationToken = default);
		Task<ProductResponse?> GetProductDetailsAsync(int productId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Deliver(Guid orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Ship(Guid orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Cancel(Guid orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> CreateOrderFromCheckoutSessionAsync(CheckoutSession checkoutSession, CancellationToken cancellationToken = default);
	}
}