using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IUserService
{
    Task<ApiResponse<List<UserManagementDto>>> GetAllUsersExceptSuperAdminAsync();
    Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId);
}