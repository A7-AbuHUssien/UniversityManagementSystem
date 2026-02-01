namespace UniversityManagementSystem.Application.DTOs.Identity_DTOs;

public class RefreshTokenRequestDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}