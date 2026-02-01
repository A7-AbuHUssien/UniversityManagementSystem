using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;

namespace UniversityManagementSystem.Application.Services.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager, 
        ITokenService tokenService, 
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return new ApiResponse<AuthResponseDto>("Invalid email or password");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);
        var refreshToken = GenerateRefreshToken();
        var userRefreshToken = new UserRefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        await _unitOfWork.Repository<UserRefreshToken>().CreateAsync(userRefreshToken);
        await _unitOfWork.CompleteAsync();
        var authData = new AuthResponseDto
        {
            Token = token,
            AcademicEmail = user.Email!,
            RefreshToken =  userRefreshToken.Token,
            Roles = roles.ToList()
        };

        return new ApiResponse<AuthResponseDto>(authData, "Login successful");
    }
    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto model)
    {
        var storedToken = (await _unitOfWork.Repository<UserRefreshToken>()
            .GetOneAsync(t => t.Token == model.RefreshToken && !t.IsRevoked));

        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
            return new ApiResponse<AuthResponseDto>("Invalid or expired refresh token");

        var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user == null) return new ApiResponse<AuthResponseDto>("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        var newJwtToken = _tokenService.GenerateJwtToken(user, roles);
        var newRefreshToken = GenerateRefreshToken();

        storedToken.IsRevoked = true;
        _unitOfWork.Repository<UserRefreshToken>().Update(storedToken);

        await _unitOfWork.Repository<UserRefreshToken>().CreateAsync(new UserRefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7)
        });

        await _unitOfWork.CompleteAsync();

        return new ApiResponse<AuthResponseDto>(new AuthResponseDto
        {
            Token = newJwtToken,
            RefreshToken = newRefreshToken,
            Expires = DateTime.UtcNow.AddHours(3),
            Roles = roles.ToList()
        }, "Token refreshed successfully");
    }
    public async Task<ApiResponse<bool>> LogOutAsync(string refreshToken)
    {
        var storedToken = (await _unitOfWork.Repository<UserRefreshToken>()
            .GetOneAsync(t => t.Token == refreshToken));

        if (storedToken == null) return new ApiResponse<bool>("Invalid token");

        storedToken.IsRevoked = true;
        _unitOfWork.Repository<UserRefreshToken>().Update(storedToken);
        await _unitOfWork.CompleteAsync();

        return new ApiResponse<bool>(true, "Logged out successfully");
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}