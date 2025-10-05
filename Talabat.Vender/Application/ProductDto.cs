namespace Talabat.Vender.Application;

public class ProductDto
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<int> GroupIds { get; set; } = new();
}

public class ModifierGroupDto
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; set; }
	public int Max { get; set; }
	public List<ModifierGroupItemDto> Data { get; set; } = new();

	public class ModifierGroupItemDto
	{
		public int ModifierId { get; set; }
		public List<int> GroupIds { get; set; } = new();
	}
}

public class ModifierDto
{
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public int Id { get; set; }
}
