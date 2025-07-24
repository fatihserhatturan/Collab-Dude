namespace AnnounceService.Domain.Events;
using AnnounceService.Domain.Entities;

public class ApplicationStatusChangedEvent : BaseDomainEvent
{
    public Guid ApplicationId { get; }
    public Guid AnnounceId { get; }
    public string ApplicantUsername { get; }
    public ApplicationStatus OldStatus { get; }
    public ApplicationStatus NewStatus { get; }

    public ApplicationStatusChangedEvent(Guid applicationId, Guid announceId, string applicantUsername, 
        ApplicationStatus oldStatus, ApplicationStatus newStatus)
    {
        ApplicationId = applicationId;
        AnnounceId = announceId;
        ApplicantUsername = applicantUsername;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}