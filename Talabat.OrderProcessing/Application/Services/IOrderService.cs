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
	}
}