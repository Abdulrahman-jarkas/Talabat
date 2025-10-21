namespace Talabat.OrderProcessing.Domain.Common;

public abstract class AggregateRoot : Entity
{
	protected AggregateRoot(int id) : base(id)
	{
	}

	protected AggregateRoot() { }

	//protected readonly List<IDomainEvent> _domainEvents = new();

	//public List<IDomainEvent> PopDomainEvents()
	//{
	//	var copy = _domainEvents.ToList();
	//	_domainEvents.Clear();

	//	return copy;
	//}
}