using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services.Identity;

public interface IPasswordService
{
    Task<ApiResponse<string>> ForgotPasswordAsync(string email);
    Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto model);
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto model);

}