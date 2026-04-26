using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Data.Repositories
{
	internal interface IOrdersRepository
	{
		Task AddAsync(Order order, CancellationToken cancellationToken = default);
		Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
		Task<Order?> GetByCheckoutSessionIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default);
		Task<int> CountOrdersTodayByShopAsync(Guid shopId, DateOnly date, CancellationToken cancellationToken = default);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}