using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services.Identity;

public interface IRegistrationService
{
    Task<ApiResponse<string>> RegisterStudentAsync(RegisterStudentDto model);
    Task<ApiResponse<string>> RegisterInstructorAsync(RegisterInstructorDto model);
}