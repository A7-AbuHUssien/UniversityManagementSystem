using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.Common;
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
    private readonly IEmailService _emailService;
    private const string UniversityDomain = "university.edu";

    public AuthService(
        UserManager<IdentityUser<Guid>> userManager, 
        ITokenService tokenService, 
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<ApiResponse<string>> RegisterStudentAsync(RegisterStudentDto model)
    {
        try
        {
            string academicEmail = $"{model.FirstName.ToLower()}.{model.LastName.ToLower()}{DateTime.Now.Year}@{UniversityDomain}";

            var user = new IdentityUser<Guid> { UserName = academicEmail, Email = academicEmail, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errorResponse = new ApiResponse<string>("Account creation failed");
                errorResponse.Errors = result.Errors.Select(e => e.Description).ToList();
                return errorResponse;
            }

            await _userManager.AddToRoleAsync(user, AppRoles.STUDENT);

            var student = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PersonalEmail = model.PersonalEmail,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                DepartmentId = model.DepartmentId,
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.Repository<Student>().CreateAsync(student);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>(academicEmail, "Student registered successfully");
        }
        catch (Exception)
        {
            throw; 
        }
    }
    public async Task<ApiResponse<string>> RegisterInstructorAsync(RegisterInstructorDto model)
    {
        try
        {
            string academicEmail = $"{model.FirstName.ToLower()}.{model.LastName.ToLower()}@{UniversityDomain}";

            var user = new IdentityUser<Guid> { UserName = academicEmail, Email = academicEmail, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errorResponse = new ApiResponse<string>("Instructor account creation failed");
                errorResponse.Errors = result.Errors.Select(e => e.Description).ToList();
                return errorResponse;
            }

            await _userManager.AddToRoleAsync(user, AppRoles.INSTRUCTOR);

            var instructor = new Instructor
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PersonalEmail = model.PersonalEmail,
                DepartmentId = model.DepartmentId,
                ApplicationUserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Instructor>().CreateAsync(instructor);
            await _unitOfWork.CompleteAsync();

            return new ApiResponse<string>(academicEmail, "Instructor registered successfully");
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return new ApiResponse<AuthResponseDto>("Invalid email or password");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateJwtToken(user, roles);

        var authData = new AuthResponseDto
        {
            Token = token,
            AcademicEmail = user.Email!,
            Expires = DateTime.UtcNow.AddHours(3),
            Roles = roles.ToList()
        };

        return new ApiResponse<AuthResponseDto>(authData, "Login successful");
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
            return new ApiResponse<string>("Error resetting password") { Errors = result.Errors.Select(e => e.Description).ToList() };

        return new ApiResponse<string>(null, "Password has been reset successfully");
    }
    public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new ApiResponse<bool>("User not found");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
            return new ApiResponse<bool>("Failed to change password") { Errors = result.Errors.Select(e => e.Description).ToList() };

        return new ApiResponse<bool>(true, "Password updated successfully");
    }
    public async Task<ApiResponse<bool>> UpdateProfileAsync(Guid userId, UpdateProfileDto model)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new ApiResponse<bool>("User not found");

        user.PhoneNumber = model.Phone;
        var identityResult = await _userManager.UpdateAsync(user);
        
        if (!identityResult.Succeeded)
            return new ApiResponse<bool>("Failed to update identity information");

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains(AppRoles.STUDENT))
        {
            var studentRepo = _unitOfWork.Repository<Student>();
            var student = (await studentRepo.GetOneAsync(s => s.ApplicationUserId == userId));
            if (student != null)
            {
                student.FirstName = model.FirstName;
                student.LastName = model.LastName;
                student.PersonalEmail = model.PersonalEmail;
                student.Phone = model.Phone;
            }
        }
        else if (roles.Contains(AppRoles.INSTRUCTOR))
        {
            var instructorRepo = _unitOfWork.Repository<Instructor>();
            var instructor = (await instructorRepo.GetOneAsync(i => i.ApplicationUserId == userId));
            if (instructor != null)
            {
                instructor.FirstName = model.FirstName;
                instructor.LastName = model.LastName;
                instructor.PersonalEmail = model.PersonalEmail;
            }
        }

        await _unitOfWork.CompleteAsync();
        return new ApiResponse<bool>(true, "Profile updated successfully");
    }
}