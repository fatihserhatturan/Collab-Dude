namespace AnnounceService.Domain.Events;

public class CommentAddedEvent : BaseDomainEvent
{
    public Guid CommentId { get; }
    public Guid AnnounceId { get; }
    public string CommenterUsername { get; }
    public string AnnounceOwnerUsername { get; }

    public CommentAddedEvent(Guid commentId, Guid announceId, string commenterUsername, string announceOwnerUsername)
    {
        CommentId = commentId;
        AnnounceId = announceId;
        CommenterUsername = commenterUsername;
        AnnounceOwnerUsername = announceOwnerUsername;
    }
}