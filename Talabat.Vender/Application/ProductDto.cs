namespace Talabat.Vender.Application;

public class ProductDto
{
	public Guid Id { get; set; } = Guid.Empty;
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<Guid> GroupIds { get; set; } = new();
}

public class ModifierGroupDto
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; set; }
	public int Max { get; set; }
	public List<ModifierGroupItemDto> Data { get; set; } = new();

	public class ModifierGroupItemDto
	{
		public Guid ModifierId { get; set; }
		public List<Guid> GroupIds { get; set; } = new();
	}
}

public class ModifierDto
{
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public Guid Id { get; set; }
}

