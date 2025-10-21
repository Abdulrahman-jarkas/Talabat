namespace Talabat.OrderProcessing.Endpoints.CreateOrder;

public class CreateOrderRequest
{
	public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
	public int ProductId { get; set; }
	public int Quantity { get; set; }
	public string Note { get; set; } = string.Empty;
	public List<Group> Groups { get; set; } = new();

	public class Group
	{
		public Guid Id { get; set; }
		public List<Modifier> Modifiers { get; set; } = new();
	}

	public class SubGroup
	{
		public Guid Id { get; set; }
		public List<int> Modifiers { get; set; } = new();
	}

	public class Modifier
	{
		public int Id { get; set; }
		public List<SubGroup> SubGroups { get; set; } = new();
	}
}