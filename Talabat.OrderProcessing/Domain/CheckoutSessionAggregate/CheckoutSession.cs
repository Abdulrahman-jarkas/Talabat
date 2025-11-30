using Talabat.OrderProcessing.Domain.Common;
using Talabat.OrderProcessing.Domain.OrderAggregate;

namespace Talabat.OrderProcessing.Domain.CheckoutSessionAggregate;

public class CheckoutSession : AggregateRoot
{
	public List<OrderItem> Items { get; private set; } = new();

	public decimal ServiceFees { get; private set; }
	public decimal Subtotal => Items.Sum(i => i.GetTotalPrice());
	public decimal Total => Subtotal + ServiceFees;

	public Guid PaymentId { get; private set; }

	public CheckoutSession(
		List<OrderItem> items,
		decimal serviceFees
		)
	{
		if (items.Count <= 0)
			throw new InvalidDataException("Checkout session items should not be empty");
		Items = items;
		ServiceFees = serviceFees;
	}

	public void SetPaymentSession(Guid paymentId)
	{
		PaymentId = paymentId;
	}
}
