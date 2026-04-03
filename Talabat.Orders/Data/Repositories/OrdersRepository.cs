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
			.Include(o => o.Items)
			.FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
	}

	public Task<Order?> GetByCheckoutSessionIdAsync(Guid checkoutSessionId, CancellationToken cancellationToken = default)
	{
		return context.Orders
			.FirstOrDefaultAsync(o => o.CheckoutSessionId == checkoutSessionId, cancellationToken);
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return context.SaveChangesAsync(cancellationToken);
	}
}
