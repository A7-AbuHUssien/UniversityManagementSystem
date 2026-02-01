using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;

namespace UniversityManagementSystem.Application.Services.Identity;

public class PasswordService : IPasswordService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IEmailService _emailService;

    public PasswordService(
        UserManager<IdentityUser<Guid>> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }
    public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return new ApiResponse<string>("If this email exists, a reset link has been sent.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var resetLink = $"https://yourfront.com/reset-password?token={token}&email={email}";
        var emailBody = $@"
        <h1>Reset Your Password</h1>
        <p>Please use the token below to reset your password:</p>
        <div style='padding:10px; background:#f3f3f3; font-weight:bold;'>{token}</div>
        <p>Or click <a href='{resetLink}'>here</a> to continue.</p>";

        await _emailService.SendEmailAsync(email, "Reset Password Token", emailBody);

        return new ApiResponse<string>(null, "A reset token has been sent to your email.");
    }

    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) return new ApiResponse<string>("Invalid Request");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
        if (!result.Succeeded)
            return new ApiResponse<string>("Error resetting password")
                { Errors = result.Errors.Select(e => e.Description).ToList() };

        return new ApiResponse<string>(null, "Password has been reset successfully");
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new ApiResponse<bool>("User not found");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
            return new ApiResponse<bool>("Failed to change password")
                { Errors = result.Errors.Select(e => e.Description).ToList() };

        return new ApiResponse<bool>(true, "Password updated successfully");
    }
}