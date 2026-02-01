using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services.Identity;

public interface IProfileService
{
    Task<ApiResponse<bool>> UpdateProfileAsync(Guid userId, UpdateProfileDto model);
}