namespace AnnounceService.Domain.Events;

public class AnnounceCreatedEvent : BaseDomainEvent
{
    public Guid AnnounceId { get; }
    public string Title { get; }
    public string Username { get; }
    public Guid CategoryId { get; }

    public AnnounceCreatedEvent(Guid announceId, string title, string username, Guid categoryId)
    {
        AnnounceId = announceId;
        Title = title;
        Username = username;
        CategoryId = categoryId;
    }
}