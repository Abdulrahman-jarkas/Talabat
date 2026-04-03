namespace Talabat.SharedKernal;

public class OutboxIntegrationEvent
{
	public Guid Id { get; init; }
	public string EventType { get; init; } = string.Empty;
	public string EventContent { get; init; } = string.Empty;
	public DateTime CreatedAtUtc { get; init; }

	public OutboxIntegrationEvent(string eventType, string eventContent)
	{
		Id = Guid.NewGuid();
		EventType = eventType;
		EventContent = eventContent;
		CreatedAtUtc = DateTime.UtcNow;
	}

	private OutboxIntegrationEvent() { }
}
