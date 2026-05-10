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

	public Task<Order?> GetByIdAsync(Guid orderId, Guid? shopId, Guid? customerId, CancellationToken cancellationToken = default)
	{
		var query = context.Orders.AsQueryable();
		if (shopId.HasValue)
			query = query.Where(o => o.ShopId == shopId.Value);
		if (customerId.HasValue)
			query = query.Where(o => o.CustomerId == customerId.Value);
		return query.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
	}

	public Task<List<Order>> GetOrdersAsync(Guid? shopId, Guid? customerId, CancellationToken cancellationToken = default)
	{
		var query = context.Orders.AsQueryable();
		if (shopId.HasValue)
			query = query.Where(o => o.ShopId == shopId.Value);
		if (customerId.HasValue)
			query = query.Where(o => o.CustomerId == customerId.Value);
		return query.ToListAsync(cancellationToken);
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
