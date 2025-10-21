namespace Talabat.Vender.Endpoints.AddProduct;

public class AddProductRequest
{
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<AddModifierGroupRequest> Groups { get; set; } = new();
}