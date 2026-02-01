using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;
public class UserService : IUserService
{
    private readonly UserManager<IdentityUser<Guid>> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(UserManager<IdentityUser<Guid>> userManager, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<UserManagementDto>>> GetAllUsersExceptSuperAdminAsync()
    {
        var users = _userManager.Users.ToList();
        var resultList = new List<UserManagementDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
        
            if (roles.Contains(AppRoles.SUPER_ADMIN)) continue;

            string userType = roles.FirstOrDefault() ?? "No Role";

            string fullName = await GetUserFullNameAsync(user.Id, roles);

            resultList.Add(new UserManagementDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = fullName,
                Roles = roles.ToList(),
                IsActive = user.EmailConfirmed,
                UserType = userType,
                JoinedAt = DateTime.UtcNow
            });
        }

        return new ApiResponse<List<UserManagementDto>>(resultList, "Users retrieved successfully");
    }

    public async Task<ApiResponse<bool>> ToggleUserStatusAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return new ApiResponse<bool>("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains(AppRoles.SUPER_ADMIN)) 
            return new ApiResponse<bool>("Cannot modify a Super Admin account");

        user.EmailConfirmed = !user.EmailConfirmed; 
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return new ApiResponse<bool>("Failed to update user status");

        return new ApiResponse<bool>(true, "User status updated successfully");
    }
    private async Task<string> GetUserFullNameAsync(Guid userId, IList<string> roles)
    {
        if (roles.Contains(AppRoles.STUDENT))
        {
            var student = (await _unitOfWork.Repository<Student>().GetOneAsync(s => s.ApplicationUserId == userId));
            return student != null ? $"{student.FirstName} {student.LastName}" : "N/A";
        }
        
        if (roles.Contains(AppRoles.INSTRUCTOR))
        {
            var instructor = (await _unitOfWork.Repository<Instructor>().GetOneAsync(i => i.ApplicationUserId == userId));
            return instructor != null ? $"{instructor.FirstName} {instructor.LastName}" : "N/A";
        }

        return "Staff/Admin";
    }
}