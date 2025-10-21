namespace Talabat.OrderProcessing.Application.DTOs;

public class OrderDetailsDto
{
	public int Id { get; set; }

	public string OrderStatus { get; set; } = string.Empty;

	public OrderPaymentDto Payment { get; set; } = default!;

	public List<OrderItemDto> Items { get; set; } = new();

	public decimal Subtotal { get; set; }
}


public class OrderPaymentDto
{
	public string PaymentStatus { get; set; } = string.Empty;
	public string PaymentMethod { get; set; } = string.Empty;
}

public class OrderItemDto
{
	public int ProductId { get; set; }
	public decimal ProductPrice { get; set; }
	public int Quantity { get; set; }
	public string Note { get; set; } = string.Empty;
	public decimal TotalPrice { get; set; }
	public List<ModifierDto> Modifiers { get; set; } = new();

	public class ModifierDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public Guid GroupId { get; set; }
		public string GroupName { get; set; } = string.Empty;
	}
}
