
namespace Talabat.Orders
{
	internal interface IOrdersRepository
	{
		Task AddAsync(Order order, CancellationToken cancellationToken = default);
		Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}