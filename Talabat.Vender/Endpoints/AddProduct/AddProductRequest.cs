using Talabat.Vender.Dto;

namespace Talabat.Vender.Endpoints.AddProduct;

public class AddProductRequest
{
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<Group> Groups { get; set; } = new();

	public class Group
	{
		public string Title { get; set; } = string.Empty;
		public int Min { get; set; }
		public int Max { get; set; }
		public List<Option> Options { get; set; } = new();
	}

	public class Option
	{
		public string Title { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public List<Group> Groups { get; set; } = new();
	}
}
