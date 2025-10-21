namespace Talabat.OrderProcessing.Domain.OrderAggregate;

public class Modifier
{
	public int Id { get; init; }
	public string Name { get; init; } = string.Empty;
	public decimal Price { get; init; }
	public Guid GroupId { get; set; }
	public string GroupTitle { get; set; } = string.Empty;
}
