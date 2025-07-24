using AutoMapper;
using AnnounceService.Application.DTOs.Application;
using AnnounceService.Application.Interfaces;
using AnnounceService.Domain.Entities;
using AnnounceService.Domain.Interfaces;

namespace AnnounceService.Application.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IAnnounceRepository _announceRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        IAnnounceRepository announceRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _applicationRepository = applicationRepository;
        _announceRepository = announceRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(Guid id)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        return application == null ? null : _mapper.Map<ApplicationDto>(application);
    }

    public async Task<IEnumerable<ApplicationDto>> GetApplicationsByAnnounceIdAsync(Guid announceId)
    {
        var applications = await _applicationRepository.GetByAnnounceIdAsync(announceId);
        return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
    }

    public async Task<IEnumerable<ApplicationDto>> GetApplicationsByUsernameAsync(string username)
    {
        var applications = await _applicationRepository.GetByUsernameAsync(username);
        return _mapper.Map<IEnumerable<ApplicationDto>>(applications);
    }

    public async Task<ApplicationDto> CreateApplicationAsync(CreateApplicationRequestDto request, string username)
    {
        // Check if announce exists and is active
        var announce = await _announceRepository.GetByIdAsync(request.AnnounceId);
        if (announce == null)
        {
            throw new InvalidOperationException("Announce not found");
        }

        if (announce.Status != AnnounceStatus.Active)
        {
            throw new InvalidOperationException("Cannot apply to inactive announce");
        }

        if (announce.Username == username)
        {
            throw new InvalidOperationException("Cannot apply to your own announce");
        }

        // Check if user already applied
        var existingApplication = await _applicationRepository.GetByAnnounceAndUsernameAsync(request.AnnounceId, username);
        if (existingApplication != null)
        {
            throw new InvalidOperationException("You have already applied to this announce");
        }

        // Check if announce is full
        if (announce.CurrentParticipants >= announce.MaxParticipants)
        {
            throw new InvalidOperationException("This announce is full");
        }

        // Check if announce is expired
        if (announce.ExpiryDate.HasValue && announce.ExpiryDate.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("This announce has expired");
        }

        // Create application
        var application = _mapper.Map<Domain.Entities.Application>(request);
        application.ApplicantUsername = username;

        var createdApplication = await _applicationRepository.AddAsync(application);

        // Send notification to announce owner
        await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationRequestDto
        {
            Username = announce.Username,
            Title = "Yeni Başvuru",
            Message = $"{username} kullanıcısı '{announce.Title}' ilanınıza başvurdu.",
            Type = NotificationType.NewApplication,
            RelatedEntityId = createdApplication.Id,
            RelatedEntityType = "Application"
        });

        return _mapper.Map<ApplicationDto>(createdApplication);
    }

    public async Task<ApplicationDto> UpdateApplicationStatusAsync(UpdateApplicationStatusRequestDto request, string reviewerUsername)
    {
        var application = await _applicationRepository.GetByIdAsync(request.Id);
        if (application == null)
        {
            throw new InvalidOperationException("Application not found");
        }

        // Get announce to check ownership
        var announce = await _announceRepository.GetByIdAsync(application.AnnounceId);
        if (announce == null)
        {
            throw new InvalidOperationException("Related announce not found");
        }

        if (announce.Username != reviewerUsername)
        {
            throw new UnauthorizedAccessException("You can only review applications for your own announces");
        }

        var oldStatus = application.Status;
        
        // Update application
        application.Status = request.Status;
        application.ReviewNote = request.ReviewNote;
        application.ReviewedAt = DateTime.UtcNow;
        application.UpdatedAt = DateTime.UtcNow;

        var updatedApplication = await _applicationRepository.UpdateAsync(application);

        // Update announce participant count if approved
        if (request.Status == ApplicationStatus.Approved && oldStatus != ApplicationStatus.Approved)
        {
            announce.CurrentParticipants++;
            await _announceRepository.UpdateAsync(announce);

            // Close announce if it's full
            if (announce.CurrentParticipants >= announce.MaxParticipants)
            {
                announce.Status = AnnounceStatus.Closed;
                await _announceRepository.UpdateAsync(announce);
            }
        }
        else if (oldStatus == ApplicationStatus.Approved && request.Status != ApplicationStatus.Approved)
        {
            // Decrease participant count if previously approved application is rejected/withdrawn
            if (announce.CurrentParticipants > 0)
            {
                announce.CurrentParticipants--;
                await _announceRepository.UpdateAsync(announce);

                // Reopen announce if it was closed due to being full
                if (announce.Status == AnnounceStatus.Closed && announce.CurrentParticipants < announce.MaxParticipants)
                {
                    announce.Status = AnnounceStatus.Active;
                    await _announceRepository.UpdateAsync(announce);
                }
            }
        }

        // Send notification to applicant
        var statusMessage = request.Status switch
        {
            ApplicationStatus.Approved => "başvurunuz onaylandı",
            ApplicationStatus.Rejected => "başvurunuz reddedildi",
            _ => "başvuru durumunuz değişti"
        };

        await _notificationService.CreateNotificationAsync(new DTOs.Notification.CreateNotificationRequestDto
        {
            Username = application.ApplicantUsername,
            Title = "Başvuru Durumu Değişti",
            Message = $"'{announce.Title}' ilanına {statusMessage}.",
            Type = NotificationType.ApplicationStatusChanged,
            RelatedEntityId = application.Id,
            RelatedEntityType = "Application"
        });

        return _mapper.Map<ApplicationDto>(updatedApplication);
    }

    public async Task DeleteApplicationAsync(Guid id, string username)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null)
        {
            throw new InvalidOperationException("Application not found");
        }

        if (application.ApplicantUsername != username)
        {
            throw new UnauthorizedAccessException("You can only delete your own applications");
        }

        // Update announce participant count if application was approved
        if (application.Status == ApplicationStatus.Approved)
        {
            var announce = await _announceRepository.GetByIdAsync(application.AnnounceId);
            if (announce != null && announce.CurrentParticipants > 0)
            {
                announce.CurrentParticipants--;
                await _announceRepository.UpdateAsync(announce);

                // Reopen announce if it was closed due to being full
                if (announce.Status == AnnounceStatus.Closed && announce.CurrentParticipants < announce.MaxParticipants)
                {
                    announce.Status = AnnounceStatus.Active;
                    await _announceRepository.UpdateAsync(announce);
                }
            }
        }

        await _applicationRepository.DeleteAsync(id);
    }

    public async Task<bool> HasUserAppliedAsync(Guid announceId, string username)
    {
        return await _applicationRepository.HasUserAppliedAsync(announceId, username);
    }

    public async Task<bool> CanUserApplyAsync(Guid announceId, string username)
    {
        var announce = await _announceRepository.GetByIdAsync(announceId);
        if (announce == null) return false;

        // Cannot apply to own announce
        if (announce.Username == username) return false;

        // Cannot apply if already applied
        if (await _applicationRepository.HasUserAppliedAsync(announceId, username)) return false;

        // Cannot apply if announce is not active
        if (announce.Status != AnnounceStatus.Active) return false;

        // Cannot apply if announce is full
        if (announce.CurrentParticipants >= announce.MaxParticipants) return false;

        // Cannot apply if announce is expired
        if (announce.ExpiryDate.HasValue && announce.ExpiryDate.Value < DateTime.UtcNow) return false;

        return true;
    }
}