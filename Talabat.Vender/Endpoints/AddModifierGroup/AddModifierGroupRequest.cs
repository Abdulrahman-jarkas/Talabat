namespace Talabat.Vender.Endpoints.AddModifierGroup;

public class AddModifierGroupRequest
{
	public string Title { get; set; } = string.Empty;
	public int Min { get; set; }
	public int Max { get; set; }
	public List<ModifierGroupItem> Items { get; set; } = new();

	public class ModifierGroupItem
	{
		public int ModifierId { get; set; }
		public List<AddModifierSubGroupRequest> SubGroups { get; set; } = new();
	}

	public class AddModifierSubGroupRequest
	{
		public string Title { get; set; } = string.Empty;
		public int Min { get; set; }
		public int Max { get; set; }
		public List<int> ModifierIds { get; set; } = new();
	}
}