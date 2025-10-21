namespace Talabat.Vender.Application;

public class ProductDetailsDto
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<GroupDetailsDto> Groups { get; set; } = new();


	public class GroupDetailsDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public int Min { get; set; }
		public int Max { get; set; }
		public List<ModifierDetailsDto> Modifiers { get; set; } = new();
	}

	public class SubGroupDetailsDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public int Min { get; set; }
		public int Max { get; set; }
		public List<ModifierDetailsForSubGroupDto> Modifiers { get; set; } = new();
	}


	public class ModifierDetailsDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public List<SubGroupDetailsDto> SubGroups { get; set; } = new();
	}

	public class ModifierDetailsForSubGroupDto
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public decimal Price { get; set; }
	}
}


