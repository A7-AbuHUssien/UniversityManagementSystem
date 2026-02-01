using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;

namespace UniversityManagementSystem.Application.Services.Identity;

public class RegistrationService : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private const string UniversityDomain = "university.edu";

    public RegistrationService(UserManager<IdentityUser<Guid>> userManager, IEmailService emailService, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }
    public async Task<ApiResponse<string>> RegisterStudentAsync(RegisterStudentDto model)
    {
        try
        {
            string academicEmail =
                $"{model.FirstName.ToLower()}.{model.LastName.ToLower()}{DateTime.Now.Year}@{UniversityDomain}";

            var user = new IdentityUser<Guid>
                { UserName = academicEmail, Email = academicEmail, EmailConfirmed = true };
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

            var user = new IdentityUser<Guid>
                { UserName = academicEmail, Email = academicEmail, EmailConfirmed = true };
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
}