using AnnounceService.Application.DTOs.Application;

namespace AnnounceService.Application.Interfaces;

public interface IApplicationService
{
    Task<ApplicationDto?> GetApplicationByIdAsync(Guid id);
    Task<IEnumerable<ApplicationDto>> GetApplicationsByAnnounceIdAsync(Guid announceId);
    Task<IEnumerable<ApplicationDto>> GetApplicationsByUsernameAsync(string username);
    Task<ApplicationDto> CreateApplicationAsync(CreateApplicationRequestDto request, string username);
    Task<ApplicationDto> UpdateApplicationStatusAsync(UpdateApplicationStatusRequestDto request, string reviewerUsername);
    Task DeleteApplicationAsync(Guid id, string username);
    Task<bool> HasUserAppliedAsync(Guid announceId, string username);
    Task<bool> CanUserApplyAsync(Guid announceId, string username);
}