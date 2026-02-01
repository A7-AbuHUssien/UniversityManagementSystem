using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;

namespace UniversityManagementSystem.Application.Services.Identity;

public class ProfileService : IProfileService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public ProfileService(UserManager<IdentityUser<Guid>> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
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