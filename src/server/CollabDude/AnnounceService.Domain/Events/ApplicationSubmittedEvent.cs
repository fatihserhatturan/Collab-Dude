namespace AnnounceService.Domain.Events;

public class ApplicationSubmittedEvent : BaseDomainEvent
{
    public Guid ApplicationId { get; }
    public Guid AnnounceId { get; }
    public string ApplicantUsername { get; }
    public string AnnounceOwnerUsername { get; }

    public ApplicationSubmittedEvent(Guid applicationId, Guid announceId, string applicantUsername, string announceOwnerUsername)
    {
        ApplicationId = applicationId;
        AnnounceId = announceId;
        ApplicantUsername = applicantUsername;
        AnnounceOwnerUsername = announceOwnerUsername;
    }
}