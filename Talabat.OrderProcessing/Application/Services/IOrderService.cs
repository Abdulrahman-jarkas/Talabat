using ErrorOr;
using Talabat.OrderProcessing.Application.DTOs;
using Talabat.OrderProcessing.Endpoints.CreateOrder;
using Talabat.ProductsManagement.Contracts;

namespace Talabat.OrderProcessing.Application.Services
{
	public interface IOrderService
	{
		Task<ErrorOr<OrderDetailsDto>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
		Task<ProductResponse?> GetProductDetailsAsync(int productId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Accept(int orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Reject(int orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Deliver(int orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Ship(int orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> Cancel(int orderId, CancellationToken cancellationToken = default);
		Task<ErrorOr<Success>> CashPay(int orderId, decimal amount, CancellationToken cancellationToken = default);
	}
}