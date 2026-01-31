namespace UniversityManagementSystem.Application.DTOs.Identity_DTOs;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}