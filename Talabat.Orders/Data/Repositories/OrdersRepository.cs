using Microsoft.EntityFrameworkCore;
using Talabat.Orders.Domain.OrderAggregate;

namespace Talabat.Orders.Data.Repositories;

internal class OrdersRepository(OrdersDbContext context) : IOrdersRepository
{

	public Task AddAsync(Order order, CancellationToken cancellationToken = default)
	{
		return context.Orders.AddAsync(order, cancellationToken).AsTask();
	}

	public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
	{
		return context.Orders
			.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
	}

	public Task<Order?> GetByCheckoutSessionIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default)
	{
		return context.Orders
			.FirstOrDefaultAsync(o => o.CheckoutSessionId == checkoutSessionId, cancellationToken);
	}

	public async Task<int> CountOrdersTodayByShopAsync(Guid shopId, DateOnly date, CancellationToken cancellationToken = default)
	{
		var startOfDay = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
		var endOfDay = date.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

		return await context.Orders
			.Where(o => o.ShopId == shopId &&
						EF.Property<DateTime>(o, "CreatedAt") >= startOfDay &&
						EF.Property<DateTime>(o, "CreatedAt") <= endOfDay)
			.CountAsync(cancellationToken);
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return context.SaveChangesAsync(cancellationToken);
	}
}
