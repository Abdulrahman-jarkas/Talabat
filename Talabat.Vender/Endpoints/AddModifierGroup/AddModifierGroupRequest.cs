namespace Talabat.Vender.Endpoints.AddModifierGroup;

public class AddModifierGroupRequest
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; set; }
	public int Max { get; set; }
	public List<ModifierGroupItem> Data { get; set; } = new();


	public class ModifierGroupItem
	{
		public Guid ModifierId { get; set; }
		public List<Guid> GroupIds { get; set; } = new();
	}
}