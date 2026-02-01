using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;
      private readonly UserManager<IdentityUser<Guid>> _userManager;

      public TokenService(IConfiguration config, IUnitOfWork unitOfWork, UserManager<IdentityUser<Guid>> userManager)
      {
          _config = config;
          _userManager = userManager;
          _unitOfWork = unitOfWork;
      } 

    public string GenerateJwtToken(IdentityUser<Guid> user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationInMinutes"]!)),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto model)
{
    // 1. فك التوكن القديم واستخراج البيانات منه (حتى لو منتهي)
    var principal = GetPrincipalFromExpiredToken(model.Token);
    if (principal == null) 
        return new ApiResponse<AuthResponseDto>("Invalid access token");

    // 2. البحث عن الـ Refresh Token في الداتابيز
    var storedToken = (await _unitOfWork.Repository<UserRefreshToken>()
        .GetOneAsync(t => t.Token == model.RefreshToken && !t.IsRevoked));

    if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        return new ApiResponse<AuthResponseDto>("Invalid or expired refresh token");

    // 3. 🛡️ الجنزير الأمني: التأكد إن التوكن والـ Refresh Token بتوع نفس الشخص
    var userIdFromToken = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userIdFromToken != storedToken.UserId.ToString())
        return new ApiResponse<AuthResponseDto>("Token mismatch! This session is suspicious.");

    var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
    if (user == null) return new ApiResponse<AuthResponseDto>("User not found");

    // ... باقي الكود بتاعك (توليد التوكينات الجديدة والحفظ)
    var roles = await _userManager.GetRolesAsync(user);
    var newJwtToken = GenerateJwtToken(user, roles); // تأكد إن دي بتنادي الميثود الصح
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
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = _config["JWT:Issuer"],
            ValidateAudience = true,
            ValidAudience = _config["JWT:Audience"],
            ValidateLifetime = false 
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}