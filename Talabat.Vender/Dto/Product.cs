namespace Talabat.Vender.Dto;

public class ProductDto
{
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<GroupDto> Groups { get; set; } = new();
}

public class GroupDto
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; set; }
	public int Max { get; set; }
	public List<OptionDto> Options { get; set; } = new();
}


public class OptionDto
{
	public string Title { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public List<GroupDto> Groups { get; set; } = new();
}