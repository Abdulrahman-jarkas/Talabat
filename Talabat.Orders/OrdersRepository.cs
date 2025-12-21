using Microsoft.EntityFrameworkCore;

namespace Talabat.Orders;

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

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return context.SaveChangesAsync(cancellationToken);
	}
}
