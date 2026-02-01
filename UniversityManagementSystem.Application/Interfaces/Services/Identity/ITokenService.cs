using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateJwtToken(IdentityUser<Guid> user, IList<string> roles);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto model);
}