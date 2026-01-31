using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services.Identity;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model);
    Task<ApiResponse<string>> RegisterStudentAsync(RegisterStudentDto model);
    Task<ApiResponse<string>> RegisterInstructorAsync(RegisterInstructorDto model);
    Task<ApiResponse<string>> ForgotPasswordAsync(string email);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto model);
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto model);
    Task<ApiResponse<bool>> UpdateProfileAsync(Guid userId, UpdateProfileDto model);
}