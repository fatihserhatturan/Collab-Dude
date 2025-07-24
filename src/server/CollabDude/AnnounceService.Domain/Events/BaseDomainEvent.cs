namespace AnnounceService.Domain.Events;

public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; protected set; }
    public Guid EventId { get; protected set; }

    protected BaseDomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}