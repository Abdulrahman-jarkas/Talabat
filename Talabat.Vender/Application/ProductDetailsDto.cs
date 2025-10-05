namespace Talabat.Vender.Application;

public class ProductDetailsDto
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	

	public class GroupDetailsDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public int Min { get; set; }
		public int Max { get; set; }
		List<ModifierDetailsDto> Modifiers { get; set; } = new();
	}

	public class ModifierDetailsDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public decimal Price { get; set; }
		List<GroupDetailsDto> Groups { get; set; } = new();
	}
}


